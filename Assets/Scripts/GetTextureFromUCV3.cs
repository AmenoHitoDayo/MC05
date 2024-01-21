using Serenegiant.UVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Serenegiant.UVC.UVCManager;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;


public class GetTextureFromUCV3 : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    [SerializeField]
    private Texture2D errorTex;
    [SerializeField]
    private int frameSpan;  //���t���[�����s���Ȃ��悤��

    private bool isCameraAttached = true;

    private RawImage rawImage;
    private List<CameraInfo> camInfos;

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
            Debug.Log("���s�t���[���̐��l���s���ł�");
            Debug.Log("���s�t���[�����l��60�ɐݒ肵�܂�");
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
                Texture2D srcTex = camInfos[0].previewTexture as Texture2D;
                Texture2D dstTex = ocvTest(srcTex);
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

    Texture2D ocvTest(Texture2D srcTex)
    {
        Mat srcMat = new Mat(srcTex.height, srcTex.width, CvType.CV_8UC3);
        Utils.texture2DToMat(srcTex, srcMat);

        Imgproc.cvtColor(srcMat, srcMat, Imgproc.COLORMAP_AUTUMN);

        Texture2D dstTex = new Texture2D(srcMat.cols(), srcMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(srcMat, dstTex);

        return dstTex;
    }
}
