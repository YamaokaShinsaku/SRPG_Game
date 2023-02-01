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
        public GameObject playerStatusWindow;
        public GameObject enemyStatusWindow;
        public Image hpGageImg;         // HP�Q�[�W�摜
        public Image EnemyHpGageImg;    // HP�Q�[�W�摜
        // �e�L�X�g
        public Text nameText;
        public Text hpText;
        public Text atkText;
        public Text defText;

        public Text EnemyNameText;
        public Text EnemyHpText;
        public Text EnemyAtkText;
        public Text EnemyDefText;

        public Text DirectionText;
        // �����A�C�R��
        public Image attributeIcon;
        public Image EnemyAttributeIcon;
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

        // �s������E�L�����Z���{�^��
        public GameObject decideButtons;

        // �J�b�g�C���摜
        public GameObject cutinPanel;
        public RawImage rawImg;

        // �����I���摜
        public GameObject directionUI;

        // Start is called before the first frame update
        void Start()
        {
            // UI������
            HidePlayerStatusWindow();
            HideEnemyStatusWindow();
            HideCommandButtons();
            HideMoveCancelButton();
            HideDecideButtons();
            HideDirectionText();

            // fadeImg�̃A���t�@�l�̏����ݒ�
            // �t�F�[�h�A�E�g����J�n���邽��
            startAlphaNum = fadeImg.color;
            startAlphaNum.a = 1.0f;
            fadeImg.color = startAlphaNum;
        }

        public void HideDirectionText()
        {
            DirectionText.enabled = false;
        }
        public void ShowDirectionText()
        {
            DirectionText.enabled = true;
        }

        /// <summary>
        /// �L�����N�^�[�̃X�e�[�^�X���\����
        /// </summary>
        /// <param name="character">�L�����N�^�[</param>
        public void HideCharaStatus(Character.Character character)
        {
            character.statusUI.SetActive(false);
            character.texture.Release();
        }

        /// <summary>
        /// �L�����N�^�[�̃X�e�[�^�X��\��
        /// </summary>
        /// <param name="character">�L�����N�^�[</param>
        public void ShowCharaStatus(Character.Character character)
        {
            character.statusUI.SetActive(true);
        }

        /// <summary>
        /// rawImage �� texture��ݒ肷��
        /// </summary>
        /// <param name="texture">�e�N�X�`��</param>
        public void SetTexture(Texture texture)
        {
            rawImg.texture = texture;
        }

        public void CutinActive()
        {
            cutinPanel.SetActive(true);
        }

        public void CutinDelete()
        {
            cutinPanel.SetActive(false);
        }

        /// <summary>
        /// �X�e�[�^�X�E�B���h�E���B��
        /// </summary>
        public void HidePlayerStatusWindow()
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            playerStatusWindow.SetActive(false);

            // �e�L�X�g���\����
            nameText.enabled = false;
            hpText.enabled = false;
            atkText.enabled = false;
            defText.enabled = false;
        }

        public void HideEnemyStatusWindow()
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            enemyStatusWindow.SetActive(false);

            // �e�L�X�g���\����
            EnemyNameText.enabled = false;
            EnemyHpText.enabled = false;
            EnemyAtkText.enabled = false;
            EnemyDefText.enabled = false;
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
        /// �s������E�L�����Z���{�^�����\����
        /// </summary>
        public void HideDecideButtons()
        {
            decideButtons.SetActive(false);
        }

        /// <summary>
        /// �X�e�[�^�X�E�B���h�E��\������
        /// </summary>
        /// <param name="charaData">�L�����N�^�[�f�[�^</param>
        public void ShowPlayerStatusWindow(Character.Character charaData)
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            playerStatusWindow.SetActive(true);

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

        public void ShowEnemyStatusWindow(Character.Character charaData)
        {
            // �I�u�W�F�N�g���A�N�e�B�u��
            enemyStatusWindow.SetActive(true);

            // �e�L�X�g��\������
            EnemyNameText.enabled = true;
            EnemyHpText.enabled = true;
            EnemyAtkText.enabled = true;
            EnemyDefText.enabled = true;

            // ���O�e�L�X�g�\��
            EnemyNameText.text = charaData.characterName;

            // �����摜�\��
            switch (charaData.attribute)
            {
                case Character.Character.Attribute.Water:
                    EnemyAttributeIcon.sprite = Water;
                    break;
                case Character.Character.Attribute.Fire:
                    EnemyAttributeIcon.sprite = Fire;
                    break;
                case Character.Character.Attribute.Wind:
                    EnemyAttributeIcon.sprite = Wind;
                    break;
                case Character.Character.Attribute.Soil:
                    EnemyAttributeIcon.sprite = Soil;
                    break;
            }

            // HP�Q�[�W�\��
            // ���݂�HP�������Q�[�W�� fillAmount �ɃZ�b�g����
            float ratio = (float)charaData.nowHP / charaData.maxHP;
            EnemyHpGageImg.fillAmount = ratio;

            // Text�\��
            EnemyHpText.text = charaData.nowHP + "/" + charaData.maxHP;
            EnemyAtkText.text = charaData.atk.ToString();
            if (!charaData.isDefBreak)
            {
                EnemyDefText.text = charaData.def.ToString();
            }
            // �h��͂O�i�f�o�t�j�̎�
            else
            {
                EnemyDefText.text = "<color=red>0</color>";
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

        public void ShowDecideButtons()
        {
            decideButtons.SetActive(true);
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