using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager.MapManager mapManager;
        [SerializeField]
        private Character.CharacterManager characterManager;
        [SerializeField]
        private UIManager.UIManager uiManager;

        // �i�s�Ǘ��p�ϐ�
        private Character.Character selectingCharacter;       // �I�𒆂̃L�����N�^�[
        private List<MapBlock> reachableBlocks;               // �I�𒆂̃L�����N�^�[�̈ړ��\�u���b�N���X�g
        private List<MapBlock> attackableBlocks;              // �I�𒆂̃L�����N�^�[�̍U���\�u���b�N���X�g
        private Character.SkillDefine.Skill selectingSkill;   // �I�𒆂̃X�L���i�ʏ�U����NONE�Œ�j

        [SerializeField]
        private bool isFinish;      // �Q�[���I���t���O

        // �x������
        const float delayTime = 0.5f;

        // �^�[���i�s���[�h
        private enum Phase
        {
            MyTurn_Start,        // �����̃^�[���F�J�n
            MyTurn_Moving,       // �����̃^�[���F�ړ���I��
            MyTurn_Command,      // �����̃^�[���F�R�}���h�I��
            MyTurn_Targeting,    // �����̃^�[���F�U���ΏۑI��
            MyTurn_Result,       // �����̃^�[���F���ʕ\��
            Enemyturn_Start,     // �G�̃^�[��  �F�J�n
            EnemyTurn_Result     // �G�̃^�[��  �F���ʕ\��
        }
        private Phase nowPhase;     // ���݂̐i�s���[�h

        // Start is called before the first frame update
        void Start()
        {
            mapManager = GetComponent<MapManager.MapManager>();
            characterManager = GetComponent<Character.CharacterManager>();
            uiManager = GetComponent<UIManager.UIManager>();

            // ���X�g��������
            reachableBlocks = new List<MapBlock>();
            attackableBlocks = new List<MapBlock>();

            // �J�n���̐i�s���[�h
            nowPhase = Phase.MyTurn_Start;

            // �t�F�[�h�A�E�g�J�n
            uiManager.StartFadeOut();

            // �t�F�[�h�A�E�g���I���������s
            DOVirtual.DelayedCall(3.5f, () =>
            {
                // �v���C���[�^�[���J�n���S�\��
                uiManager.ShowPlayerTurnLogo();
            });
        }

        // Update is called once per frame
        void Update()
        {
            // �Q�[�����I�����Ă���Ȃ�
            if(isFinish)
            {
                return;
            }

            // �^�b�v������o
            if(Input.GetMouseButtonDown(0) 
                // UI�ւ̃^�b�v�����o����
                && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // �f�o�b�O�p����
                // �o�g�����ʃE�B���h�E���\������Ă���Ƃ�
                if(uiManager.battleWindowUI.gameObject.activeInHierarchy)
                {
                    // �o�g�����ʃE�B���h�E�����
                    uiManager.battleWindowUI.HideWindow();

                    // �i�s���[�h��i�߂�
                    ChangePhase(Phase.MyTurn_Start);

                    return;
                }
                GetMapObjects();
            }
        }

        /// <summary>
        /// �Q�[���I�������𖞂����Ă��邩�m�F���A�������Ă���ΏI������
        /// </summary>
        public void CheckFinish()
        {
            // �v���C���[�����t���O
            bool isWin = true;
            // �v���C���[�s�k�t���O
            bool isLose = true;

            // �����Ă��閡���A�G�����݂��邩���`�F�b�N
            foreach(var charaData in characterManager.characters)
            {
                // �G������Ƃ�
                if(charaData.isEnemy)
                {
                    // �����t���O��false
                    isWin = false;
                }
                // ����������Ƃ�
                else
                {
                    // �s�k�t���O��false
                    isLose = false;
                }
            }

            // �����A�s�k�t���O�̂ǂ��炩��true�Ȃ�Q�[�����I��
            if(isWin || isLose)
            {
                // �Q�[���I���t���O��true
                isFinish = true;

                // ���S�摜��\��
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    // �Q�[���N���A�̎�
                    if (isWin)
                    {
                        uiManager.ShowGameClearLogo();
                    }
                    // �Q�[���I�[�o�[�̎�
                    else
                    {
                        uiManager.ShowGameOverLogo();
                    }
                    // �t�F�[�h�C���J�n
                    uiManager.StartFadeIn();
                });

                // EnhanceScene�̓ǂݍ���
                DOVirtual.DelayedCall(7.0f, () =>
                {
                    SceneManager.LoadScene("Enhance");
                });
            }
        }

        /// <summary>
        /// �^�b�v�����ꏊ�̃I�u�W�F�N�g���擾
        /// </summary>
        private void GetMapObjects()
        {
            GameObject targetObject = null;     // �^�b�v�����I�u�W�F�N�g

            // �^�b�v���������ɃJ��������Ray���΂�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            // Ray�̓�����ʒu�ɑ��݂���I�u�W�F�N�g���擾
            if(Physics.Raycast(ray, out hit))
            {
                targetObject = hit.collider.gameObject;
            }

            // �ΏۃI�u�W�F�N�g�����݂���ꍇ�̏���
            if(targetObject != null)
            {
                SelectBlock(targetObject.GetComponent<MapBlock>());
            }
        }

        /// <summary>
        /// �w�肵���u���b�N��I����Ԃɂ���
        /// </summary>
        /// <param name="targetBlock">�Ώۂ̃I�u�W�F�N�g�f�[�^</param>
        private void SelectBlock(MapBlock targetObject)
        {
            // �i�s���[�h���Ƃɏ����𕪊򂷂�
            switch(nowPhase)
            {
                // �����̃^�[�� �F �J�n
                case Phase.MyTurn_Start:
                    // �S�u���b�N�̑I����Ԃ���������
                    mapManager.AllSelectionModeClear();
                    // �u���b�N��I����Ԃɂ���
                    targetObject.SetSelectionMode(MapBlock.Highlight.Select);

                    Debug.Log("�I�u�W�F�N�g���^�b�v����܂��� \n�u���b�N���W : "
                        + targetObject.transform.position);

                    // �I�������ʒu�ɂ���L�����N�^�[�f�[�^���擾
                    var charaData =
                        characterManager.GetCharacterData(targetObject.xPos, targetObject.zPos);
                    // �L�����N�^�[�����݂���Ƃ�
                    if (charaData != null)
                    {
                        Debug.Log("�L�����N�^�[�����݂��܂� : "
                            + charaData.gameObject.name);

                        // �I�𒆂̃L�����N�^�[���ɋL������
                        selectingCharacter = charaData;
                        // �L�����N�^�[�̃X�e�[�^�XUI��\������
                        uiManager.ShowPlayerStatusWindow(selectingCharacter);
                        // �ړ��\�ȏꏊ���X�g���擾����
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // �ړ��\�ȏꏊ���X�g��\������
                        foreach(MapBlock mapBlock in reachableBlocks)
                        {
                            mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                        }

                        // �ړ��L�����Z���{�^����\��
                        uiManager.ShowMoveCancelButton();
                        // �i�s���[�h��i�߂�
                        ChangePhase(Phase.MyTurn_Moving);
                    }
                    // �L�����N�^�[�����݂��Ȃ��Ƃ�
                    else
                    {
                        Debug.Log("�L�����N�^�[�͑��݂��܂���");

                        // �I�𒆂̃L�����N�^�[��������������
                        ClearSelectingChara();
                    }
                    break;
                // �����̃^�[�� : �ړ�
                case Phase.MyTurn_Moving:
                    // �G�L�����N�^�[��I�𒆂Ȃ�ړ����L�����Z�����ďI��
                    if(selectingCharacter.isEnemy)
                    {
                        CancelMoving();
                        break;
                    }

                    // �I�������u���b�N���ړ��\�ȏꏊ���X�g���ɂ���Ƃ�
                    if(reachableBlocks.Contains(targetObject))
                    {
                      // �I�𒆂̃L�����N�^�[���ړ�������
                        selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);

                        // �ړ��\�ȏꏊ���X�g������������
                        reachableBlocks.Clear();
                        // �S�u���b�N�̑I����Ԃ�����
                        mapManager.AllSelectionModeClear();
                        // �ړ��L�����Z���{�^�����\����
                        uiManager.HideMoveCancelButton();

                        // �w�莞�Ԍ�ɏ��������s����
                        DOVirtual.DelayedCall(delayTime, () =>
                            {
                                // �x�����s������e
                                // �R�}���h�{�^����\������
                                uiManager.ShowCommandButtons(selectingCharacter);

                                // �i�s���[�h��i�߂�
                                ChangePhase(Phase.MyTurn_Command);
                            });
                    }
                    break;
                // �����̃^�[�� : �R�}���h�I��
                case Phase.MyTurn_Command:
                    // �U������
                    // �U���\�u���b�N��I�������ꍇ�ɍU���������Ă�
                    // �U���\�u���b�N���^�b�v������
                    if (attackableBlocks.Contains(targetObject))
                    {
                        // �U���\�ȏꏊ���X�g������������
                        attackableBlocks.Clear();
                        // �S�u�b�N�̑I����Ԃ�����
                        mapManager.AllSelectionModeClear();

                        // �U���Ώۂ̈ʒu�ɂ���L�����N�^�[�f�[�^���擾
                        var targetChara =
                            characterManager.GetCharacterData(targetObject.xPos, targetObject.zPos);
                        // �U���Ώۂ̃L�����N�^�[�����݂���Ƃ�
                        if (targetChara != null)
                        {
                            // �L�����N�^�[�U������
                            Attack(selectingCharacter, targetChara);

                            // �i�s���[�h��i�߂�
                            ChangePhase(Phase.MyTurn_Result);
                            return;
                        }
                        // �U���Ώۂ����݂��Ȃ��Ƃ�
                        else
                        {
                            // �i�s���[�h��i�߂�
                            ChangePhase(Phase.Enemyturn_Start);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// �^�[���i�s���[�h��ύX
        /// </summary>
        /// <param name="newPhase">�ύX��̃��[�h</param>
        /// <param name="noLogos">���S��\���t���O</param>
        private void ChangePhase(Phase newPhase, bool noLogos = false)
        {
            // �Q�[�����I�����Ă���Ȃ�
            if (isFinish)
            {
                return;
            }

            // �i�s���[�h��ύX
            nowPhase = newPhase;

            // ����̃��[�h�ɐ؂�ւ�����^�C�~���O�ōs������
            switch (nowPhase)
            {
                // �����̃^�[�� : �J�n
                case Phase.MyTurn_Start:
                    if(!noLogos)
                    {
                        // ���S�摜��\��
                        uiManager.ShowPlayerTurnLogo();
                    }
                    break;
                // �G�̃^�[�� : �J�n
                case Phase.Enemyturn_Start:
                    if (!noLogos)
                    {
                        // ���S�摜��\��
                        uiManager.ShowEnemyTurnLogo();
                    }

                    // �G�̍s���������J�n����(�x�����s)
                    DOVirtual.DelayedCall(delayTime, () =>
                    {
                        EnemyCommand();
                    });
                    break;
            }
        }

        /// <summary>
        /// �I�𒆂̃L�����N�^�[��񏉊�������
        /// </summary>
        private void ClearSelectingChara()
        {
            // �I�𒆂̃L�����N�^�[������������
            selectingCharacter = null;
            // �L�����N�^�[�̃X�e�[�^�X��UI���\���ɂ���
            uiManager.HidePlayerStatusWindow();
        }

        /// <summary>
        /// �U���R�}���h�{�^������
        /// </summary>
        public void AttackCommand()
        {
            // �X�L���̑I����Ԃ��I�t�ɂ���
            selectingSkill = Character.SkillDefine.Skill.None;
            // �U���͈͂��擾���ĕ\��
            GetAttackableBlocks();

            //// �R�}���h�{�^�����\���ɂ���
            //uiManager.HideCommandButtons();

            //// �U���͈͎擾
            //// �U���\�ȏꏊ���X�g���擾����
            //attackableBlocks = mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            //// �U���\�ȏꏊ���X�g��\������
            //foreach(MapBlock mapBlock in attackableBlocks)
            //{
            //    mapBlock.SetSelectionMode(MapBlock.Highlight.Attackable);
            //}
        }

        /// <summary>
        /// �X�L���R�}���h�{�^������
        /// </summary>
        public void SkillCommand()
        {
            // �L�����N�^�[�̎��X�L����I����Ԃɂ���
            selectingSkill = selectingCharacter.skill;
            // �U���͈͂��擾���ĕ\��
            GetAttackableBlocks();
        }

        /// <summary>
        /// �U���E�X�L���R�}���h�I�����ɑΏۃu���b�N��\������
        /// </summary>
        private void GetAttackableBlocks()
        {
            // �R�}���h�{�^�����\���ɂ���
            uiManager.HideCommandButtons();
            // �U���\�ȏꏊ���X�g���擾����
            // �X�L�� : �t�@�C�A�{�[���̏ꍇ�̓}�b�v�S��ɑΉ�
            if(selectingSkill == Character.SkillDefine.Skill.FireBall)
            {
                attackableBlocks = mapManager.MapBlocksToList();
            }
            else
            {
                attackableBlocks =
                    mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            }

            // �U���\�ȏꏊ���X�g��\��
            foreach(MapBlock mapBlock in attackableBlocks)
            {
                mapBlock.SetSelectionMode(MapBlock.Highlight.Attackable);
            }
        }

        /// <summary>
        /// �ҋ@����
        /// </summary>
        public void StandCommand()
        {
            // �R�}���h�{�^�����\����
            uiManager.HideCommandButtons();
            // �i�s���[�h��i�߂�
            ChangePhase(Phase.Enemyturn_Start);
        }

        /// <summary>
        /// �I�𒆂̃L�����N�^�[�̈ړ����͑҂���Ԃ���������
        /// </summary>
        public void CancelMoving()
        {
            // �S�u���b�N�̑I����Ԃ�����
            mapManager.AllSelectionModeClear();
            // �ړ��\�ȏꏊ���X�g��������
            reachableBlocks.Clear();
            // �ړ��L�����Z���{�^�����\����
            uiManager.HideMoveCancelButton();
            // �i�s���[�h�����ɖ߂�
            ChangePhase(Phase.MyTurn_Start, true);
        }

        /// <summary>
        /// �U������
        /// </summary>
        /// <param name="attackChara">�U������L�����N�^�[</param>
        /// <param name="defenseChara">�U�������L�����N�^�[</param>
        private void Attack(Character.Character attackChara, Character.Character defenseChara)
        {
            Debug.Log("�U���� : " + attackChara.characterName
                + "  �h�䑤 : " + defenseChara.characterName);

            // �_���[�W�v�Z
            int damageValue;    // �_���[�W��
            int attackPoint = attackChara.atk;     // �U�����鑤�̍U����
            int defencePoint = defenseChara.def;   // �U������鑤�̖h���

            // �h��͂O�i�f�o�t�j���������Ă��鎞
            if(defenseChara.isDefBreak)
            {
                defencePoint = 0;
            }

            // �_���[�W�@���@�U���́@�[�@�h���
            damageValue = attackPoint - defencePoint;
            // ���������ɂ��_���[�W�{�����v�Z
            float ratio = GetDamageRatioAttribute(attackChara, defenseChara);       // �_���[�W�{�����擾
            damageValue = (int)(damageValue * ratio);       // �{����K��(int�^�ɕϊ�)

            // �_���[�W�ʂ�0�ȉ��̎�
            if (damageValue < 0)
            {
                // 0�ɂ���
                damageValue = 0;
            }

            // �I�������X�L���ɂ��_���[�W�l�␳�ƌ��ʏ���
            switch(selectingSkill)
            {
                //�N���e�B�J���i��S�̈ꌂ�j
                case Character.SkillDefine.Skill.Critical:
                    // �_���[�W2�{
                    damageValue *= 2;
                    // �X�L���g�p�s�\��Ԃ�
                    attackChara.isSkillLock = true;
                    break;
                // �V�[���h�j��
                case Character.SkillDefine.Skill.DefBreak:
                    // �_���[�W0�Œ�
                    damageValue = 0;
                    // �h��͂O�i�f�o�t�j���Z�b�g
                    defenseChara.isDefBreak = true;
                    break;
                // �q�[��
                case Character.SkillDefine.Skill.Heal:
                    // �񕜁i�񕜗ʂ͍U���͂̔����j
                    damageValue = (int)(attackPoint * -0.5f);
                    break;
                // �t�@�C�A�{�[��
                case Character.SkillDefine.Skill.FireBall:
                    // �^����_���[�W����
                    damageValue /= 2;
                    break;
                // �X�L������ or �ʏ�U��
                default:
                    break;
            }

            // �U���A�j���[�V����
            // �q�[���E�t�@�C�A�{�[���̓A�j���[�V��������
            if(selectingSkill != Character.SkillDefine.Skill.Heal
                && selectingSkill != Character.SkillDefine.Skill.FireBall)
            {
                attackChara.AttackAnimation(defenseChara);
            }
            // �U�������������^�C�~���O��SE���Đ�
            DOVirtual.DelayedCall(0.45f, () =>
                {
                    GetComponent<AudioSource>().Play();
                });

            // �o�g�����ʕ\���E�B���h�E�̕\���ݒ�
            uiManager.battleWindowUI.ShowWindow(defenseChara, damageValue);

            // �_���[�W�ʕ��U�����ꂽ����HP������������
            defenseChara.nowHP -= damageValue;
            // HP��0�`�ő�l�͈̔͂Ɏ��܂�悤�ɕ␳
            defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

            // HP��0�ɂȂ����L�����N�^�[���폜����
            if(defenseChara.nowHP == 0)
            {
                characterManager.DeleteCharaData(defenseChara);
            }

            // �X�L���̑I����Ԃ���������
            selectingSkill = Character.SkillDefine.Skill.None;

            // �^�[���؂�ւ�����
            DOVirtual.DelayedCall(2.0f, () =>
            {
                // �x�����s������e
                // �E�B���h�E���\����
                uiManager.battleWindowUI.HideWindow();

                // �^�[����؂�ւ���
                if (nowPhase == Phase.MyTurn_Result)
                {
                    // �G�̃^�[����
                    ChangePhase(Phase.Enemyturn_Start);
                }
                else if (nowPhase == Phase.EnemyTurn_Result)
                {
                    // �����̃^�[����
                    ChangePhase(Phase.MyTurn_Start);
                }
            });

        }

        /// <summary>
        /// �G�L�����N�^�[�̂�����̂��s�������ă^�[�����I������
        /// </summary>
        private void EnemyCommand()
        {
            // �X�L���̑I����Ԃ��I�t�ɂ���
            selectingSkill = Character.SkillDefine.Skill.None;

            // �������̓G�L�����N�^�[�̃��X�g���쐬
            // �G�L�����N�^�[���X�g
            var enemyCharacters = new List<Character.Character>();
            foreach(Character.Character charaData in characterManager.characters)
            {
                // �S�����L�����N�^�[����AEnemy�t���O �������Ă�����̂����X�g�ɒǉ�
                if(charaData.isEnemy)
                {
                    enemyCharacters.Add(charaData);
                }
            }

            // �U���\�ȃL�����N�^�[�E�ʒu�̑g�ݍ��킹��1�����_���Ɏ擾
            var actionPlan = TargetFinder.GetRandomactionPlans(mapManager, characterManager, enemyCharacters);
            // �g�ݍ��킹�̃f�[�^�����݂���΁A�U������
            if(actionPlan != null)
            {
                // �ړ�����
                actionPlan.charaData.MovePosition(actionPlan.toMoveBlock.xPos, actionPlan.toMoveBlock.zPos);
                // �U������(�ړ���ɍU���J�n)
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    Attack(actionPlan.charaData, actionPlan.toAttaackChara);
                });

                // �i�s���[�h��i�߂�
                ChangePhase(Phase.EnemyTurn_Result);
                return;
            }

            // �U���\�ȃL�����N�^�[��������Ȃ�������
            // �ړ�������L�����N�^�[����̃����_���ɑI��
            int randID = Random.Range(0, enemyCharacters.Count);
            //  �s������G�L�����N�^�[�̃f�[�^���擾
            Character.Character targetEnemy = enemyCharacters[randID];
            // �Ώۂ̈ړ��\�ȏꏊ���X�g�̂Ȃ�����P�̏ꏊ�������_���ɑI��
            reachableBlocks =
                mapManager.SearchReachableBlocks(targetEnemy.xPos, targetEnemy.zPos);
            if(reachableBlocks.Count > 0)
            {
                randID = Random.Range(0, reachableBlocks.Count);
                // �ړ���̃u���b�N�f�[�^
                MapBlock targetBlock = reachableBlocks[randID];
                // �ړ�����
                targetEnemy.MovePosition(targetBlock.xPos, targetBlock.zPos);
            }

            // ���X�g���N���A
            reachableBlocks.Clear();
            attackableBlocks.Clear();

            // �i�s���[�h��i�߂�
            ChangePhase(Phase.MyTurn_Start);
        }

        /// <summary>
        /// ���������ɂ��_���[�W�{����Ԃ�
        /// </summary>
        /// <param name="attackChara">�U������L�����N�^�[�f�[�^</param>
        /// <param name="defenseChara">�U�������L�����N�^�[�f�[�^</param>
        /// <returns>�_���[�W�{��</returns>
        private float GetDamageRatioAttribute(Character.Character attackChara, Character.Character defenseChara)
        {
            // �_���[�W�{�����`
            const float normal = 1.0f;     // �ʏ�
            const float good = 1.2f;        // �L��
            const float bad = 0.8f;          // �s��

            Character.Character.Attribute attacker = attackChara.attribute;        // �U�����̑���
            Character.Character.Attribute defender = defenseChara.attribute;     // �h�䑤�̑���

            // �������菈��
            // �������ƂɗL�����s���̏��Ń`�F�b�N���A�ǂ���ɂ����Ă͂܂�Ȃ���Βʏ�{����Ԃ�
            switch(attacker)
            {
                // �U���� : ������
                case Character.Character.Attribute.Water:
                    if(defender == Character.Character.Attribute.Fire)
                    {
                        return good;
                    }
                    else if( defender == Character.Character.Attribute.Soil)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // �U���� : �Α���
                case Character.Character.Attribute.Fire:
                    if (defender == Character.Character.Attribute.Wind)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Water)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // �U���� : ������
                case Character.Character.Attribute.Wind:
                    if (defender == Character.Character.Attribute.Soil)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Fire)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // �U���� : �y����
                case Character.Character.Attribute.Soil:
                    if (defender == Character.Character.Attribute.Water)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Wind)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // �f�t�H���g�ݒ�
                default:
                    return normal;
            }
        }
    }
}