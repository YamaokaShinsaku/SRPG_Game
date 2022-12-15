using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataManager
{
    /// <summary>
    /// �V�[������1�̂ݑ��݂���
    /// </summary>
    public class Data : MonoBehaviour
    {
        // �V���O���g���Ǘ��p�ϐ�
        public static bool instance = false;

        private void Awake()
        {
            // �V���O���g���p����
            // ���ł�DataManager���V�[�����ɑ��݂��Ă���ꍇ�͎���������
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }

            // DataManager���������ꂽ���Ƃ�static�ϐ��ɋL�^
            instance = true;
            // �V�[�����܂����ł����̃I�u�W�F�N�g����������Ȃ��悤�ɂ���
            DontDestroyOnLoad(this.gameObject);
        }
    }

}