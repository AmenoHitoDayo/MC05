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
        camInfos = uvc.GetAttachedDevices();
        if (camInfos.Count > 0)
        {
            rawImage.texture = camInfos[0].previewTexture;
        }
        else
        {
            if (rawImage.material.mainTexture != errorTex)
            {
                rawImage.texture = errorTex;
                Debug.Log("no camera found");
            }
        }

    }
}
