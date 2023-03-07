using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    // 次のシーンの名前
    public string nextScene;

    public float delayTime;
    public AudioManager audioManager;

    // フェードイン用画像
    public Image fadeImg;
    Color startAlphaNum;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = audioManager.GetComponent<AudioManager>();

        // fadeImgのアルファ値の初期設定
        // フェードアウトから開始するため
        startAlphaNum = fadeImg.color;
        startAlphaNum.a = 0.0f;
        fadeImg.color = startAlphaNum;
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName">次のシーンの名前</param>
    public void ChangeScene(string sceneName)
    {
        audioManager.Diceid();
        StartFadeIn();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // シーン遷移
            SceneManager.LoadScene(sceneName);
        });
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    public void StartFadeIn()
    {
        fadeImg.DOFade(
            1.0f,       // 指定数値まで画像のα値を変化させる
            2.0f)       // アニメーション時間
            .SetEase(Ease.Linear);
    }
}
