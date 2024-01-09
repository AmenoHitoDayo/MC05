using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class ocvfuTest01 : MonoBehaviour
{
    [SerializeField]
    private Texture2D srcTex;
    [SerializeField]
    private RawImage dstRawImage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % 60 == 0)
        {
            Mat srcMat = new Mat(srcTex.height, srcTex.width, CvType.CV_8UC3);
            Utils.texture2DToMat(srcTex, srcMat);

            Imgproc.cvtColor(srcMat, srcMat, Imgproc.COLOR_RGBA2GRAY);

            Texture2D dstTex = new Texture2D(srcMat.cols(), srcMat.rows(), TextureFormat.ARGB32, false);
            Utils.matToTexture2D(srcMat, dstTex);

            dstRawImage.texture = dstTex;
        }
    }
}
