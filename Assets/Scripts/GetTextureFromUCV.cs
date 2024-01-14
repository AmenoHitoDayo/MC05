using Serenegiant.UVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTextureFromUCV : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    UVCManager.CameraInfo cameraInfo;
    private Texture2D tex2d;

    [SerializeField]
    private Renderer render;
    [SerializeField]
    private Texture2D defaultTex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(uvc.GetAttachedDevices().Count > 0)
        {
            cameraInfo = uvc.GetAttachedDevices()[0];
            tex2d = cameraInfo.previewTexture as Texture2D;
            render.material.mainTexture = tex2d;
        }
        else
        {
            if(render.material.mainTexture != defaultTex)
                render.material.mainTexture = defaultTex;
            return;
        }

    }
}
