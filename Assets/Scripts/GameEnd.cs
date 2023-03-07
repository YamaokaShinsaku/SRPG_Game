using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void FinishGame()
    {
#if UNITY_EDITOR
        //ゲームプレイ終了
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //ゲームプレイ終了
    Application.Quit();
#endif
    }
}
