using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    /// <summary>
    /// �Q�[�����I������
    /// </summary>
    public void FinishGame()
    {
#if UNITY_EDITOR
        //�Q�[���v���C�I��
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //�Q�[���v���C�I��
    Application.Quit();
#endif
    }
}
