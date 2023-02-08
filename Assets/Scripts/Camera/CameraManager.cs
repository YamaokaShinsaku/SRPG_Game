using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    // ���C���J����
    public GameObject mainCamera;

    // �g�p����J����
    public GameObject[] cameras;

    // �N���b�N������
    public int count;

    // Start is called before the first frame update
    void Start()
    {
        ShowCamera(mainCamera);
        HideCamera(cameras[0]);
        HideCamera(cameras[1]);
        HideCamera(cameras[2]);
        HideCamera(cameras[3]);

        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            count++;
        }

        if(count >= 5)
        {
            count = 0;
        }

        ChangeCamera(count);
    }

    public void AddCount()
    {
        count++;

        if (count >= 5)
        {
            count = 0;
        }
    }

    public void DecreaseCount()
    {
        count--;

        switch(count)
        {
            case 0:
                count = 0;
                break;
            case -1:
                count = 4;
                break;
            case -2:
                count = 3;
                break;
            case -3:
                count = 2;
                break;
            case -4:
                count = 1;
                break;
        }
    }

    /// <summary>
    /// �J�������\���ɂ���
    /// </summary>
    /// <param name="camera">�J����</param>
    public void HideCamera(GameObject camera)
    {
        camera.SetActive(false);
    }

    /// <summary>
    /// �J������\������
    /// </summary>
    /// <param name="camera">�J����</param>
    public void ShowCamera(GameObject camera)
    {
        camera.SetActive(true);
    }

    /// <summary>
    /// �J������؂�ւ���
    /// </summary>
    /// <param name="nowCount">�N���b�N��</param>
    public void ChangeCamera(int nowCount)
    {
        switch (count)
        {
            case 0:
                HideCamera(cameras[4]);
                ShowCamera(cameras[0]);

                break;
            case 1:
                HideCamera(cameras[0]);
                ShowCamera(cameras[1]);

                break;
            case 2:
                HideCamera(cameras[1]);
                ShowCamera(cameras[2]);

                break;
            case 3:
                HideCamera(cameras[2]);
                ShowCamera(cameras[3]);

                break;
            case 4:
                HideCamera(cameras[3]);
                ShowCamera(cameras[4]);

                break;
        }
    }
}