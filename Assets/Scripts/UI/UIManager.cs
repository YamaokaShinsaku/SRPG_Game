using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class UIManager : MonoBehaviour
    {
        /// �X�e�[�^�X�E�B���h�E ///
        public GameObject statusWindow;
        public Image hpGageImg;         // HP�Q�[�W�摜
                                        // �e�L�X�g
        public Text nameText;
        public Text hpText;
        public Text atkText;
        public Text defText;
        // �����A�C�R��
        public Image attributeIcon;
        public Sprite Water;    // ��
        public Sprite Fire;     // ��
        public Sprite Wind;     // ��
        public Sprite Soil;     // �y

        // �R�}���h�{�^��
        public GameObject commandButtons;   // �S�R�}���h�{�^���̐e�I�u�W�F�N�g

        public BattleWindowUI battleWindowUI;

        // Start is called before the first frame update
        void Start()
        {
            // UI������
            HideStatusWindow();
            HideCommandButtons();
        }

        /// <summary>
        /// �X�e�[�^�X�E�B���h�E���B��
        /// </summary>
        public void HideStatusWindow()
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            statusWindow.SetActive(false);

            // �e�L�X�g���\����
            nameText.enabled = false;
            hpText.enabled = false;
            atkText.enabled = false;
            defText.enabled = false;
        }

        /// <summary>
        /// �R�}���h�{�^�����\����
        /// </summary>
        public void HideCommandButtons()
        {
            commandButtons.SetActive(false);
        }

        /// <summary>
        /// �X�e�[�^�X�E�B���h�E��\������
        /// </summary>
        /// <param name="charaData">�L�����N�^�[�f�[�^</param>
        public void ShowStatusWindow(Character.Character charaData)
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            statusWindow.SetActive(true);

            // �e�L�X�g��\������
            nameText.enabled = true;
            hpText.enabled = true;
            atkText.enabled = true;
            defText.enabled = true;

            // ���O�e�L�X�g�\��
            nameText.text = charaData.characterName;

            // �����摜�\��
            switch(charaData.attribute)
            {
                case Character.Character.Attribute.Water:
                    attributeIcon.sprite = Water;
                    break;
                case Character.Character.Attribute.Fire:
                    attributeIcon.sprite = Fire;
                    break;
                case Character.Character.Attribute.Wind:
                    attributeIcon.sprite = Wind;
                    break;
                case Character.Character.Attribute.Soil:
                    attributeIcon.sprite = Soil;
                    break;
            }

            // HP�Q�[�W�\��
            // ���݂�HP�������Q�[�W�� fillAmount �ɃZ�b�g����
            float ratio = (float)charaData.nowHP / charaData.maxHP;
            hpGageImg.fillAmount = ratio;

            // Text�\��
            hpText.text = charaData.nowHP + "/" + charaData.maxHP;
            atkText.text = charaData.atk.ToString();
            defText.text = charaData.def.ToString();
        }

        /// <summary>
        /// �R�}���h�{�^����\��
        /// </summary>
        public void ShowCommandButtons()
        {
            commandButtons.SetActive(true);
        }
    }
}