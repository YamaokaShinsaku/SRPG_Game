using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    // ���̃V�[���̖��O
    public string nextScene;

    public float delayTime;
    public AudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        audioManager = audioManager.GetComponent<AudioManager>();
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="sceneName">���̃V�[���̖��O</param>
    public void ChangeScene(string sceneName)
    {
        audioManager.Diceid();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // �V�[���J��
            SceneManager.LoadScene(sceneName);
        });
    }

}
