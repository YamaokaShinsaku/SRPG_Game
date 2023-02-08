using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChange : MonoBehaviour
{
    // 次のシーンの名前
    public string nextScene;

    public float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        delayTime = 0.0f;
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName">次のシーンの名前</param>
    public void ChangeScene(string sceneName)
    {
        // フェードスタート
        //uiManager.StartFadeIn();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // シーン遷移
            SceneManager.LoadScene(sceneName);
        });
    }

}
