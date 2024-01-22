using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textureOutController : MonoBehaviour
{
    private RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        // 右コントローラのX Buttonが押された時にtrue
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            // 処理
            rawImage.enabled = !rawImage.enabled;
        }
    }
}
