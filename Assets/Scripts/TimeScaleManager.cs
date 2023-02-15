using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TimeScale�𐧌䂷�邽�߂̃N���X
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    public bool isPause;    // ��~
    public GameObject monoTone;    // ���m�g�[���V�F�[�_�[

    private void Start()
    {
        monoTone.SetActive(false);
    }

    void Update()
    {


    }

    /// <summary>
    /// ��~������
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0.0f;
        monoTone.SetActive(true);
    }

    /// <summary>
    /// �ĊJ����
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
        monoTone.SetActive(false);
    }
}