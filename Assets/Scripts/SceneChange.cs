using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChange : MonoBehaviour
{
    // ���̃V�[���̖��O
    public string nextScene;

    public float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        delayTime = 0.0f;
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="sceneName">���̃V�[���̖��O</param>
    public void ChangeScene(string sceneName)
    {
        // �t�F�[�h�X�^�[�g
        //uiManager.StartFadeIn();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // �V�[���J��
            SceneManager.LoadScene(sceneName);
        });
    }

}
