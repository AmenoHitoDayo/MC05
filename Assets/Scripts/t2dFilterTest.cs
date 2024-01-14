using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class t2dFilterTest : MonoBehaviour
{
    [SerializeField]
    private Texture2D originTex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void filter01()
    {
        Color[] pixels = originTex.GetPixels();

        Color[] filted_pixels = new Color[pixels.Length];
        for(int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            Color filted_pixel = new Color(1f, pixel.g, pixel.b, pixel.a);
            filted_pixels.SetValue(filted_pixel, i);
        }

        Texture2D filted_texture = new Texture2D(originTex.width, originTex.height, TextureFormat.ARGB32, false);
        filted_texture.filterMode = FilterMode.Point;
        filted_texture.SetPixels(filted_pixels);
        filted_texture.Apply();

        GetComponent<Renderer>().material.mainTexture = filted_texture;
    }
}
