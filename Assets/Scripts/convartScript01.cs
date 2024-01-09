using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UtilsModule;

public class convartScript01 : MonoBehaviour
{
    [SerializeField]
    private RawImage input;
    [SerializeField]
    private RawImage output;

    [SerializeField]
    int fps = 60;

    [SerializeField, Header("ピクセルサイズ:割り切れる数じゃないとエラー吐くっぽい？")]
    int pixSize = 20;

    Texture2D srcTexture;
    Texture2D dstTexture;

    [SerializeField]
    private bool isMondOut = true;

    List<MyRect> myRects;

    //色の定義
    private double[] red = { 255, 10, 10 };
    private double[] yellow = { 255, 255, 10 };
    private double[] blue = { 10, 10, 255 };
    private double[] black = { 40, 40, 40 };
    private double[] white = { 220, 220, 220 };

    // Start is called before the first frame update
    void Start()
    {
        //pixSize = Gcd(width, height);

        myRects = new List<MyRect>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.frameCount % Mathf.Round(60/fps) == 0)
        {
            //Debug.Log(Time.frameCount);

            srcTexture = (Texture2D)input.texture as Texture2D;

            Mat srcMat = new Mat(this.srcTexture.height, this.srcTexture.width, CvType.CV_8UC3);
            Mat dstMat = new Mat(this.srcTexture.height, this.srcTexture.width, CvType.CV_8UC3, new Scalar(255, 255, 255));

            if(this.dstTexture == null)
            {
                Debug.Log("tex created");
                dstTexture = new Texture2D(srcMat.cols(), srcMat.rows(), TextureFormat.RGBA32, false);
            }

            Utils.texture2DToMat(this.srcTexture, srcMat);

            
            makeMozike2(srcMat);
            pentatomization(srcMat);
            //makeMonds(srcMat, dstMat);

            //Utils.matToTexture2D(dstMat, dstTexture);
            
            Utils.matToTexture2D(srcMat, dstTexture);

            output.texture = dstTexture;

            srcMat.Dispose();
            dstMat.Dispose();
        }

        //破棄したほうが良いぽい？
        //WebCamTexture.Destroy(webCamTexture);
        //srcMat.Dispose();
        //Texture2D.Destroy(dstTexture);
    }

    void makeMonds(Mat srcMat, Mat dstMat)
    {
        myRects.Clear();

        int width2 = srcMat.width();
        int height2 = srcMat.height();
        int channel = srcMat.channels();

        byte[] data = new byte[srcMat.width() * srcMat.height() * channel];
        MatUtils.copyFromMat(srcMat, data);

        for (int i = 0; i < width2; i += pixSize)
        {
            for (int j = 0; j < height2; j += pixSize)
            {
                makeRect(data, width2, height2, channel,  i, j);
            }
        }


        foreach (MyRect r in myRects)
        {
            r.drawMe(dstMat);
        }
    }

    void makeRect(byte[] data, int matWid, int matHei, int channel, int x, int y)
    {
        int mxWid = matWid - x;
        int mxHei = matHei - y;

        int wid = 0, hei = 0;

        foreach (MyRect r in myRects){
            if (r.isIn(x, y)) return;

            int ex = r.xDist(x, y);
            int why = r.yDist(x, y);
            if(ex > 0 && ex < mxWid) { mxWid = ex; }
            if(why > 0 && why < mxHei) { mxHei = why; }
        }

        int idxx = (x + y * matWid);
        MyColor myC = new MyColor(data[idxx + 0], data[idxx + 1], data[idxx + 2]);

        while(wid < mxWid)
        {
            MyColor pointColor;
            int idx = ((x + wid) + y * matWid) * channel;
            pointColor = new MyColor(data[idx + 0], data[idx + 1], data[idx + 2]);
            if (!myC.compare(pointColor))
            {
                Debug.Log("different color ditected!");
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
                Debug.Log("different color ditected!");
                break;
            }
            hei += pixSize;
        }

        
        if(wid > 0 && hei > 0)
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

        for(int y = 0; y < hei; y += pixSize)
        {
            for(int x = 0; x < wid; x += pixSize)
            {
                int basIDX = (x + y * wid) * cha;
                byte mkr = data[basIDX + 0],
                    mkg = data[basIDX + 1],
                    mkb = data[basIDX + 2];

                for(int xx = 0; xx < pixSize; xx++)
                {
                    for(int yy = 0; yy < pixSize; yy++)
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
    void pentatomization(Mat material)
    {
        Imgproc.cvtColor(material, material, Imgproc.COLOR_RGB2HSV_FULL);

        int wid = material.width();
        int hei = material.height();
        int chan = material.channels();

        byte[] data = new byte[wid * hei * chan];
        MatUtils.copyFromMat(material, data);

        for(int y = 0; y < hei; y++)
        {
            for(int x = 0; x < wid; x++)
            {
                int idx = (x + y * wid) * chan;
                byte hue = data[idx + 0],
                    sat = data[idx + 1],
                    bri = data[idx + 2];
                byte oR = 255,
                    oG = 255,
                    oB = 255;

                if(sat > 50)
                {
                    if(hue < 15 || hue > 200)
                    {
                        //red
                        oR = 255;
                        oG = 10;
                        oB = 10;
                    }else if(hue >= 15 && hue <= 90)
                    {
                        //yellow
                        oR = 255;
                        oG = 255;
                        oB = 10;
                    }
                    else
                    {
                        //blue
                        oR = 10;
                        oG = 10;
                        oB = 255;
                    }
                }
                else
                {
                    if(bri > 127)
                    {
                        //white
                        oR = 220;
                        oG = 220;
                        oB = 220;
                    }
                    else
                    {
                        //brack
                        oR = 40;
                        oG = 40;
                        oB = 40;
                    }
                }

                data[idx + 0] = oR;
                data[idx + 1] = oG;
                data[idx + 2] = oB;
            }
        }

        MatUtils.copyToMat(data, material);
    }

    void makeMozike(Mat material)
    {

        for (int x = 0; x < material.rows(); x += pixSize)
        {
            for (int y = 0; y < material.cols(); y += pixSize)
            {
                double[] data = material.get(x, y);
                double[] result = white;

                if(data[1] > 50)
                {
                    if (data[0] < 15 || data[0] > 200)
                        result = red;
                    else if (data[0] >= 15 && data[0] <= 90)
                        result = yellow;
                    else
                        result = blue;
                }
                else
                {
                    if (data[2] > 127)
                        result = white;
                    else
                        result = black;
                }

                for (int xx = 0; xx < pixSize; xx++)
                {
                    for (int yy = 0; yy < pixSize; yy++)
                    {
                        material.put(x + xx, y + yy, result);
                    }
                }
            }
        }
    }


    //最大公約数求めるやts
    public static int Gcd(int a, int b)
    {
        return a > b ? GcdRecursive(a, b) : GcdRecursive(b, a);
    }

    private static int GcdRecursive(int a, int b)
    {
        return b == 0 ? a : GcdRecursive(b, a % b);
    }
}
