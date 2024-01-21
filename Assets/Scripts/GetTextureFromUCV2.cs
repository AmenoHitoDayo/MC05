using Serenegiant.UVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Serenegiant.UVC.UVCManager;

public class GetTextureFromUCV2 : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    [SerializeField]
    private Texture2D errorTex;
    [SerializeField]
    private int frameSpan;  //毎フレーム実行しないように

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
            Debug.Log("実行フレームの数値が不正です");
            Debug.Log("実行フレーム数値を60に設定します");
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
                rawImage.texture = camInfos[0].previewTexture;
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
}
