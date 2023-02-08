using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChange : MonoBehaviour
{
    private UIManager.UIManager uiManager;
    // 次のシーンの名前
    public string nextScene;

    // Start is called before the first frame update
    void Start()
    {
        // コンポーネント取得
        uiManager = GetComponent<UIManager.UIManager>();
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName">次のシーンの名前</param>
    public void ChangeScene(string sceneName)
    {
        // フェードスタート
        uiManager.StartFadeIn();
        DOVirtual.DelayedCall(7.0f, () =>
        {
            // シーン遷移
            SceneManager.LoadScene(sceneName);
        });
    }

}
