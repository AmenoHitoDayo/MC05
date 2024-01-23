using Serenegiant.UVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Serenegiant.UVC.UVCManager;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using UnityEngine.Rendering;
using OpenCVForUnity.UtilsModule;

public class GetTextureFromUCV3 : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    [SerializeField]
    private Texture2D errorTex;
    [SerializeField]
    private int frameSpan = 60;  //???t???[?????s????????????

    [SerializeField]
    private int pixSize = 32;

    List<MyRect> myRects;

    Texture2D srcTex;
    Texture2D dstTex;

    private bool isCameraAttached = true;

    private RawImage rawImage;
    private List<CameraInfo> camInfos;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        myRects = new List<MyRect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frameSpan <= 0)
        {
            Debug.Log("frameSpanの値が不正です");
            Debug.Log("60フレーム毎に実行するよう設定します");
            frameSpan = 60;
            return;
        }

        if (Time.frameCount % frameSpan == 0)
        {
            camInfos = uvc.GetAttachedDevices();
            if (camInfos.Count > 0)
            {
                if (!isCameraAttached)
                {
                    Debug.Log("Camera found:" + camInfos[0].DeviceName);
                    isCameraAttached = true;
                }
                srcTex = make2Dtex(camInfos[0].previewTexture);

                Mat srcMat = new Mat(this.srcTex.height, this.srcTex.width, CvType.CV_8UC3);
                Mat dstMat = new Mat(this.srcTex.height, this.srcTex.width, CvType.CV_8UC3, new Scalar(255, 255, 255));

                if (this.dstTex == null)
                {
                    Debug.Log("tex created");
                    dstTex = new Texture2D(srcMat.cols(), srcMat.rows(), TextureFormat.RGBA32, false);
                }

                Utils.texture2DToMat(this.srcTex, srcMat);


                makeMozike2(srcMat);
                pentatomization(srcMat);
                makeMonds(srcMat, dstMat);
                Utils.matToTexture2D(dstMat, dstTex);

                rawImage.texture = dstTex;

                srcMat.Dispose();
                dstMat.Dispose();

                /*
                //rawImage.texture = srcTex;
                Texture2D dstTex = ocvTest(srcTex);
                rawImage.texture = dstTex;
                */
            }
            else
            {
                if (rawImage.material.mainTexture != errorTex)
                {
                    if (isCameraAttached)
                    {
                        isCameraAttached = false;
                        rawImage.texture = errorTex;
                        Debug.Log("no camera found");
                    }
                }
            }
        }
    }

    Texture2D make2Dtex(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
        // mainTexture ???s?N?Z???????? renderTexture ???R?s?[
        Graphics.Blit(texture, renderTexture);

        // renderTexture ???s?N?Z???????????? texture2D ???s?N?Z????????????
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        Color[] pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;

        return texture2D;
    }

    void makeMonds(Mat srcMat, Mat dstMat)
    {
        myRects.Clear();

        int width2 = srcMat.width();
        int height2 = srcMat.height();
        int channel = srcMat.channels();

        byte[] data = new byte[srcMat.width() * srcMat.height() * channel];
        MatUtils.copyFromMat(srcMat, data);

        for (int y = 0; y < height2; y += pixSize)
        {
            for (int x = 0; x < width2; x += pixSize)
            {
                makeRect(data, width2, height2, channel, x, y);
            }
        }

        foreach (MyRect r in myRects)
        {
            Debug.Log("pos(" + r.X + "," + r.Y + "), size(" + r.W + "," + r.H + ")");
            r.drawMe(dstMat, pixSize);
        }

    }

    void makeRect(byte[] data, int matWid, int matHei, int channel, int x, int y)
    {
        //作れる最大サイズの矩形の縦横幅を設定
        int mxWid = matWid - x;
        int mxHei = matHei - y;

        int wid = 0, hei = 0;

        foreach (MyRect r in myRects)
        {
            //矩形の右下座標が自分よりも左上にあるなら、その矩形との判定はTE飛ばしていい
            if (r.X + r.W < x && r.Y + r.H < y) return;

            //すでに矩形の中にあるならreturn
            if (r.isIn(x + 1, y + 1)) return;

            int ex = r.xDist(x, y);
            int why = r.yDist(x, y);
            if (ex > 0 && ex < mxWid) { mxWid = ex; }
            if (why > 0 && why < mxHei) { mxHei = why; }
        }

        //mat配列内で何番目に(x,y)の画素があるか
        int idxx = (x + y * matWid);
        MyColor myC = new MyColor(data[idxx + 0], data[idxx + 1], data[idxx + 2]);

        while (wid < mxWid)
        {
            MyColor pointColor;
            int idx = ((x + wid) + y * matWid) * channel;
            pointColor = new MyColor(data[idx + 0], data[idx + 1], data[idx + 2]);
            if (!myC.compare(pointColor))
            {
                Debug.Log("different color ditected!:x");
                break;
            }
            wid += pixSize;
        }

        while (hei < mxHei)
        {
            MyColor pointColor;
            int idx = (x + (hei + y) * matWid) * channel;
            pointColor = new MyColor(data[idx + 0], data[idx + 1], data[idx + 2]);
            if (!myC.compare(pointColor))
            {
                Debug.Log("different color ditected!:y");
                break;
            }
            hei += pixSize;
        }


        if (wid > 0 && hei > 0)
        {
            MyRect r = new MyRect(x, y, wid, hei);
            r.setColor(myC);
            myRects.Add(r);
        }

    }

    //やった！！！！！！天才だ！！！！！！！！！
    //copyFromMatで、matデータを直接byte配列で取れる。
    //それを操作して、copyToMatすることで、高速にピクセル操作が可能
    //byte配列は、matの幅*高さ*チャンネルの長さが必要
    void makeMozike2(Mat material)
    {
        int wid = material.width();
        int hei = material.height();
        int cha = material.channels();

        byte[] data = new byte[wid * hei * cha];
        MatUtils.copyFromMat(material, data);
        Debug.Log("a" + data[0].ToString());

        for (int y = 0; y < hei; y += pixSize)
        {
            for (int x = 0; x < wid; x += pixSize)
            {
                int basIDX = (x + y * wid) * cha;
                byte mkr = data[basIDX + 0],
                    mkg = data[basIDX + 1],
                    mkb = data[basIDX + 2];

                //これないとoutofboundする（割り切れなかず入れると配列外に出ちゃう）
                int xRim = Mathf.Min(pixSize, wid - x);
                int yRim = Mathf.Min(pixSize, hei - y);

                for (int xx = 0; xx < xRim; xx++)
                {
                    for (int yy = 0; yy < yRim; yy++)
                    {
                        int idx = (x + xx + (y + yy) * wid) * cha;
                        if (idx > cha * wid * hei) break;
                        data[idx + 0] = mkr;
                        data[idx + 1] = mkb;
                        data[idx + 2] = mkg;
                    }
                }
            }
        }

        MatUtils.copyToMat(data, material);
    }


    //画像の5色化
    //OpenCVのHSVは、0→180で赤→緑→青→赤ではなく、赤→青→緑→赤（逆方向）になってるっぽい？
    void pentatomization(Mat material)
    {
        Imgproc.cvtColor(material, material, Imgproc.COLOR_RGB2HSV);

        int wid = material.width();
        int hei = material.height();
        int chan = material.channels();

        byte[] data = new byte[wid * hei * chan];
        MatUtils.copyFromMat(material, data);

        for (int y = 0; y < hei; y++)
        {
            for (int x = 0; x < wid; x++)
            {
                int idx = (x + y * wid) * chan;
                byte hue = data[idx + 0],
                    sat = data[idx + 1],
                    bri = data[idx + 2];
                byte oR = 255,
                    oG = 255,
                    oB = 255;

                if (sat > 50)
                {
                    if (hue < 10 || hue > 170)
                    {
                        //red
                        oR = 255;
                        oG = 10;
                        oB = 10;
                    }
                    else if (hue >= 10 && hue <= 90)
                    {
                        //blue
                        oR = 10;
                        oG = 10;
                        oB = 255;
                    }
                    else
                    {
                        //yellow
                        oR = 255;
                        oG = 255;
                        oB = 10;
                    }

                }
                else
                {
                    if (bri > 127)
                    {
                        //white
                        oR = 255;
                        oG = 255;
                        oB = 255;
                    }
                    else
                    {
                        //brack
                        oR = 42;
                        oG = 42;
                        oB = 42;
                    }
                }

                data[idx + 0] = oR;
                data[idx + 1] = oG;
                data[idx + 2] = oB;
            }
        }

        MatUtils.copyToMat(data, material);
    }

    Texture2D ocvTest(Texture2D srcTex)
    {
        Mat srcMat = new Mat(srcTex.height, srcTex.width, CvType.CV_8UC3);
        Utils.texture2DToMat(srcTex, srcMat);

        Imgproc.cvtColor(srcMat, srcMat, Imgproc.COLOR_RGBA2GRAY);

        Texture2D dstTex = new Texture2D(srcMat.cols(), srcMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(srcMat, dstTex);

        return dstTex;
    }

    Texture2D tex2DTest(Texture2D srcTex)
    {
        int width = srcTex.width;
        int height = srcTex.height;

        Color[] inputColors = srcTex.GetPixels();
        Color[] outputColors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var color = inputColors[(width * y) + x];
                outputColors[(width * y) + x] = new Color(color.g, color.b, color.r);
            }
        }
        Texture2D dstTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        dstTexture.SetPixels(outputColors);
        dstTexture.Apply();

        return dstTexture;
    }
}
