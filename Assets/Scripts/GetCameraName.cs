using Serenegiant.UVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Serenegiant.UVC.UVCManager;

public class GetCameraName : MonoBehaviour
{
    [SerializeField]
    private UVCManager uvc;
    private Text text;
    private List<CameraInfo> camInfo;
    private string cameraName;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        camInfo = uvc.GetAttachedDevices();
        if(camInfo.Count > 0)
        {
            cameraName = camInfo[0].DeviceName;
        }
        else
        {
            cameraName = "none";
        }
        text.text = cameraName;
        
    }
}
