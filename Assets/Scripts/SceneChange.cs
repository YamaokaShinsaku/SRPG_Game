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


    // Start is called before the first frame update
    void Start()
    {
        audioManager = audioManager.GetComponent<AudioManager>();
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName">次のシーンの名前</param>
    public void ChangeScene(string sceneName)
    {
        audioManager.Diceid();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // シーン遷移
            SceneManager.LoadScene(sceneName);
        });
    }

}
