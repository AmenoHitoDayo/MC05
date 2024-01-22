using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ocvTestFilter01 : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private int executeTic = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(executeTic <= 0)
        {
            Debug.Log("executeTicの値が不正です");
            executeTic = 60;
            Debug.Log("executeTicを" + executeTic +"に設定しました");
            return;
        }

        if(Time.frameCount % executeTic == 0)
        {

        }
    }
}
