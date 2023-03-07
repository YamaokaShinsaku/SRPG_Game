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

    // �t�F�[�h�C���p�摜
    public Image fadeImg;
    Color startAlphaNum;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = audioManager.GetComponent<AudioManager>();

        // fadeImg�̃A���t�@�l�̏����ݒ�
        // �t�F�[�h�A�E�g����J�n���邽��
        startAlphaNum = fadeImg.color;
        startAlphaNum.a = 0.0f;
        fadeImg.color = startAlphaNum;
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="sceneName">���̃V�[���̖��O</param>
    public void ChangeScene(string sceneName)
    {
        audioManager.Diceid();
        StartFadeIn();
        DOVirtual.DelayedCall(delayTime, () =>
        {
            // �V�[���J��
            SceneManager.LoadScene(sceneName);
        });
    }

    /// <summary>
    /// �t�F�[�h�C���J�n
    /// </summary>
    public void StartFadeIn()
    {
        fadeImg.DOFade(
            1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
            2.0f)       // �A�j���[�V��������
            .SetEase(Ease.Linear);
    }
}
