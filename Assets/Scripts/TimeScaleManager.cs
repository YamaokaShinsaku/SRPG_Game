using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TimeScaleを制御するためのクラス
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    public bool isPause;    // 停止
    //public bool isResume;   // 再開

    private void Start()
    {

    }

    void Update()
    {
        // デバッグ処理
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    isPause = true;
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    isPause = false;
        //}

        //if (isPause)
        //{
        //    Pause();
        //}
        //else
        //{
        //    Resume();
        //}

    }

    /// <summary>
    /// 停止させる
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// 再開する
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
    }
}