using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        public GameObject commandButtons;       // �S�R�}���h�{�^���̐e�I�u�W�F�N�g
        public GameObject moveCancelButton;     // �ړ��L�����Z���{�^��
        public Button skillCommandButton;       // �X�L���R�}���h�̃{�^��

        // �X�L�������p�e�L�X�g
        public Text skillText;

        // �o�g�����ʕ\���E�B���h�E
        public BattleWindowUI battleWindowUI;

        // ���S�摜
        public Image playerTurnImg;
        public Image enemyTurnImg;
        public Image gameClearImg;
        public Image gameOverImg;

        // �t�F�[�h�C���p�摜
        public Image fadeImg;
        Color startAlphaNum;

        // Start is called before the first frame update
        void Start()
        {
            // UI������
            HideStatusWindow();
            HideCommandButtons();
            HideMoveCancelButton();

            // fadeImg�̃A���t�@�l�̏����ݒ�
            // �t�F�[�h�A�E�g����J�n���邽��
            startAlphaNum = fadeImg.color;
            startAlphaNum.a = 1.0f;
            fadeImg.color = startAlphaNum;
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
            skillCommandButton.gameObject.SetActive(false);

            skillText.enabled = false;
        }

        /// <summary>
        /// �ړ��L�����Z���{�^�����\����
        /// </summary>
        public void HideMoveCancelButton()
        {
            moveCancelButton.SetActive(false);
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
            if(!charaData.isDefBreak)
            {
                defText.text = charaData.def.ToString();
            }
            // �h��͂O�i�f�o�t�j�̎�
            else
            {
                defText.text = "<color=red>0</color>";
            }
        }

        /// <summary>
        /// �R�}���h�{�^����\��
        /// </summary>
        /// <param name="selectChara">�I�𒆂̃L�����N�^�[</param>
        public void ShowCommandButtons(Character.Character selectChara)
        {
            commandButtons.SetActive(true);
            skillCommandButton.gameObject.SetActive(true);

            skillText.enabled = true;

            // �I�𒆂̃L�����N�^�[�̃X�L����Text�ɕ\������
            // �I�𒆂̃L�����N�^�[�̃X�L��
            Character.SkillDefine.Skill skill = selectChara.skill;
            // �X�L����
            string skillName = Character.SkillDefine.dicSkillName[skill];
            // �X�L���̐�����
            string skillInfo = Character.SkillDefine.dicSkillInfo[skill];

            // ���b�`�e�L�X�g�ŃT�C�Y��ύX���Ȃ��當����\��
            skillText.text = "<size=300>" + skillName + "</size>\n" + skillInfo;

            // �X�L���g�p�s�\�Ȃ�X�L���{�^���������Ȃ��悤�ɂ���
            if(selectChara.isSkillLock)
            {
                skillCommandButton.interactable = false;
            }
            else
            {
                skillCommandButton.interactable = true;
            }

        }

        /// <summary>
        /// �ړ��L�����Z���{�^����\��
        /// </summary>
        public void ShowMoveCancelButton()
        {
            moveCancelButton.SetActive(true);
        }

        /// <summary>
        /// �v���C���[�^�[���̃��S��\������
        /// </summary>
        public void ShowPlayerTurnLogo()
        {
            // ���X�ɕ\���E��\�����s��
            playerTurnImg.DOFade(
                1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                1.0f)       // �A�j���[�V��������
                .SetEase(Ease.OutCubic)             // �ω��̓x������ݒ�
                .SetLoops(2, LoopType.Yoyo);     // ���[�v�񐔁E������ݒ�
        }

        /// <summary>
        /// �G�l�~�[�^�[���̃��S��\������
        /// </summary>
        public void ShowEnemyTurnLogo()
        {
            // ���X�ɕ\���E��\�����s��
            enemyTurnImg.DOFade(
                1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                1.0f)       // �A�j���[�V��������
                .SetEase(Ease.OutCubic)             // �ω��̓x������ݒ�
                .SetLoops(2, LoopType.Yoyo);     // ���[�v�񐔁E������ݒ�
        }

        /// <summary>
        /// �Q�[���N���A�摜��\��
        /// </summary>
        public void ShowGameClearLogo()
        {
            // ���X�ɕ\���E��\�����s��
            gameClearImg.DOFade(
                1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                1.0f)       // �A�j���[�V��������
                .SetEase(Ease.OutCubic);            // �ω��̓x������ݒ�

            // �g��E�k���A�j���[�V����
            gameClearImg.transform.DOScale(1.5f, 1.0f)
                .SetEase(Ease.OutCubic)
                .SetLoops(2, LoopType.Yoyo);
        }

        /// <summary>
        /// �Q�[���I�[�o�[�摜��\��
        /// </summary>
        public void ShowGameOverLogo()
        {
            // ���X�ɕ\���E��\�����s��
            gameOverImg.DOFade(
                1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                1.0f)       // �A�j���[�V��������
                .SetEase(Ease.OutCubic);            // �ω��̓x������ݒ�
        }

        /// <summary>
        /// �t�F�[�h�C���J�n
        /// </summary>
        public void StartFadeIn()
        {
            fadeImg.DOFade(
                1.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                3.5f)       // �A�j���[�V��������
                .SetEase(Ease.Linear);
        }

        /// <summary>
        /// �t�F�[�h�A�E�g�J�n
        /// </summary>
        public void StartFadeOut()
        {
            fadeImg.DOFade(
                0.0f,       // �w�萔�l�܂ŉ摜�̃��l��ω�������
                3.5f)       // �A�j���[�V��������
                .SetEase(Ease.Linear);
        }
    }
}