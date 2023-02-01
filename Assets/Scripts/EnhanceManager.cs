using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DataManager
{
    /// <summary>
    /// Data�N���X�̏㏸�ʂ̒l�̕ύX������
    /// </summary>
    public class EnhanceManager : MonoBehaviour
    {
        // Data�N���X
        private Data data;

        // UI�{�^��
        public List<Button> EnhanceButtons;    // �X�e�[�^�X�㏸�{�^��
        public Button GoGameButton;            // ������x�v���C�{�^��

        // Start is called before the first frame update
        void Start()
        {
            // DataManager����f�[�^�Ǘ��N���X���擾
            data = GameObject.Find("DataManager").GetComponent<Data>();

            // �����ꂩ����������܂ł́u������x�v���C�{�^���v�������Ȃ��悤�ɂ���
            GoGameButton.interactable = false;
        }

        /// <summary>
        /// �ő�HP���㏸������
        /// </summary>
        public void AddHP()
        {
            // ��������
            data.addHP += 2;

            // �����I�����̏���
            EnhanceComplete();
        }

        /// <summary>
        /// �U���͂��㏸������
        /// </summary>
        public void AddAtk()
        {
            // ��������
            data.addAtk += 1;

            // �����I�����̏���
            EnhanceComplete();
        }

        /// <summary>
        /// �h��͂��㏸������
        /// </summary>
        public void AddDef()
        {
            // ��������
            data.addDef += 1;

            // �����I�����̏���
            EnhanceComplete();
        }

        /// <summary>
        /// �����������̋��ʏ���
        /// </summary>
        private void EnhanceComplete()
        {
            // �����{�^���������Ȃ��悤�ɂ���
            foreach(Button button in EnhanceButtons)
            {
                button.interactable = false;
            }

            // ������x�v���C�{�^����������悤�ɂ���
            GoGameButton.interactable = true;

            // �ύX���f�[�^�ɕۑ�
            data.WriteSaveData();
        }

        /// <summary>
        /// �Q�[���V�[���ɐ؂�ւ���
        /// </summary>
        public void GoGameScene()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
