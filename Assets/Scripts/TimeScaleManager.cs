using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TimeScaleを制御するためのクラス
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    public bool isPause;    // 停止
    public GameObject monoTone;    // モノトーンシェーダー

    private void Start()
    {
        monoTone.SetActive(false);
    }

    void Update()
    {


    }

    /// <summary>
    /// 停止させる
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0.0f;
        monoTone.SetActive(true);
    }

    /// <summary>
    /// 再開する
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
        monoTone.SetActive(false);
    }
}