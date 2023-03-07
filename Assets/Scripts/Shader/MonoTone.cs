using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoTone : MonoBehaviour
{
    [SerializeField]
    private Material monoTone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, monoTone);
    }
}
