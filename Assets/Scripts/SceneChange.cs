using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChange : MonoBehaviour
{
    private UIManager.UIManager uiManager;
    // ���̃V�[���̖��O
    public string nextScene;

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g�擾
        uiManager = GetComponent<UIManager.UIManager>();
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="sceneName">���̃V�[���̖��O</param>
    public void ChangeScene(string sceneName)
    {
        // �t�F�[�h�X�^�[�g
        uiManager.StartFadeIn();
        DOVirtual.DelayedCall(7.0f, () =>
        {
            // �V�[���J��
            SceneManager.LoadScene(sceneName);
        });
    }

}
