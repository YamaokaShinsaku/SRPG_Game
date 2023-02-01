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
        // ���ׂẴC���X�^���X�ł��̒l�����L����
        [HideInInspector]
        public static bool instance = false;

        // �v���C���[�����f�[�^
        public int addHP;    // �ő�HP�㏸��
        public int addAtk;   // �U���͏㏸��
        public int addDef;   // �h��͏㏸��

        // �f�[�^�̃L�[�ݒ�
        public const string Key_AddHP = "Key_AddHP";
        public const string Key_AddAtk = "Key_AddAtk";
        public const string Key_AddDef = "Key_AddDef";

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

            // �Z�[�u�f�[�^��PlayerPrefs����ǂݍ���
            // �L�[�ɑΉ�����f�[�^��ǂݍ���
            // �f�[�^���Ȃ��ꍇ�́A�ݒ肵�������l(0)������
            addHP = PlayerPrefs.GetInt(Key_AddHP, 0);
            addAtk = PlayerPrefs.GetInt(Key_AddAtk, 0);
            addDef = PlayerPrefs.GetInt(Key_AddDef, 0);
        }

        /// <summary>
        /// ���݂̃v���C���[�����f�[�^��PlayerPrefs�ɕۑ�����
        /// </summary>
        public void WriteSaveData()
        {
            // �f�[�^���L�[�ƂƂ��ɕύX
            PlayerPrefs.SetInt(Key_AddHP, addHP);
            PlayerPrefs.SetInt(Key_AddAtk, addAtk);
            PlayerPrefs.SetInt(Key_AddDef, addDef);
            // �ύX��ۑ�
            PlayerPrefs.Save();
        }
    }

}