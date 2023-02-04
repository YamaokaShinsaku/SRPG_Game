using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    [SerializeField, Range(1, 30)]
    private int _iteration = 1;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _threshold = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _softThreshold = 0.0f;
    [SerializeField, Range(0.0f, 10.0f)]
    private float _intensity = 1.0f;
    [SerializeField]
    private bool _debug;

    // 4点をサンプリングして色を作るマテリアル
    [SerializeField]
    private Material _material;

    private RenderTexture[] _renderTextures = new RenderTexture[30];

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        var filterParams = Vector4.zero;
        var knee = _threshold * _softThreshold;
        filterParams.x = _threshold;
        filterParams.y = _threshold - knee;
        filterParams.z = knee * 2.0f;
        filterParams.w = 0.25f / (knee + 0.00001f);
        _material.SetVector("_FilterParams", filterParams);
        _material.SetFloat("_Intensity", _intensity);
        _material.SetTexture("_SourceTex", source);

        var width = source.width;
        var height = source.height;
        var currentSource = source;

        var pathIndex = 0;
        var i = 0;
        RenderTexture currentDest = null;

        // ダウンサンプリング
        for (; i < _iteration; i++)
        {
            width /= 2;
            height /= 2;
            if (width < 2 || height < 2)
            {
                break;
            }
            currentDest = _renderTextures[i] = RenderTexture.GetTemporary(width, height, 0, source.format);

            // 最初の一回は明度抽出用のパスを使ってダウンサンプリングする
            pathIndex = i == 0 ? 0 : 1;
            Graphics.Blit(currentSource, currentDest, _material, pathIndex);

            currentSource = currentDest;
        }

        // アップサンプリング
        for (i -= 2; i >= 0; i--)
        {
            currentDest = _renderTextures[i];

            // Blit時にマテリアルとパスを指定する
            Graphics.Blit(currentSource, currentDest, _material, 2);

            _renderTextures[i] = null;
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }

        // 最後にdestにBlit
        pathIndex = _debug ? 4 : 3;
        Graphics.Blit(currentSource, dest, _material, pathIndex);
        RenderTexture.ReleaseTemporary(currentSource);
    }
}
