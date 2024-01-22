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

public class GetTextureFromUCV3 : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    [SerializeField]
    private Texture2D errorTex;
    [SerializeField]
    private int frameSpan = 60;  //???t???[?????s????????????

    private bool isCameraAttached = true;

    private RawImage rawImage;
    private List<CameraInfo> camInfos;

    private void Awake()
    {
        Application.targetFrameRate = 60;
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
            Debug.Log("???s?t???[???????l???s??????");
            Debug.Log("???s?t???[?????l??60????????????");
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
                Texture2D srcTex = make2Dtex(camInfos[0].previewTexture);
                //rawImage.texture = srcTex;
                Texture2D dstTex = tex2DTest(srcTex);
                rawImage.texture = dstTex;
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

    Texture2D ocvTest(Texture2D srcTex)
    {
        Mat srcMat = new Mat(srcTex.height, srcTex.width, CvType.CV_8UC3);
        Utils.texture2DToMat(srcTex, srcMat);

        Imgproc.cvtColor(srcMat, srcMat, Imgproc.COLORMAP_AUTUMN);

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
