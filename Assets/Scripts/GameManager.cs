using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private Character.Character selectingCharacter;   // �I�𒆂̃L�����N�^�[
        private List<MapBlock> reachableBlocks;           // �I�𒆂̃L�����N�^�[�̈ړ��\�u���b�N���X�g
        private List<MapBlock> attackableBlocks;          // �I�𒆂̃L�����N�^�[�̍U���\�u���b�N���X�g

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
        }

        // Update is called once per frame
        void Update()
        {
            // �^�b�v������o
            if(Input.GetMouseButtonDown(0))
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
                        uiManager.ShowStatusWindow(selectingCharacter);
                        // �ړ��\�ȏꏊ���X�g���擾����
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // �ړ��\�ȏꏊ���X�g��\������
                        foreach(MapBlock mapBlock in reachableBlocks)
                        {
                            mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                        }

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
                    // �I�������u���b�N���ړ��\�ȏꏊ���X�g���ɂ���Ƃ�
                    if(reachableBlocks.Contains(targetObject))
                    {
                      // �I�𒆂̃L�����N�^�[���ړ�������
                        selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);

                        // �ړ��\�ȏꏊ���X�g������������
                        reachableBlocks.Clear();
                        // �S�u���b�N�̑I����Ԃ�����
                        mapManager.AllSelectionModeClear();

                        // �w�莞�Ԍ�ɏ��������s����
                        DOVirtual.DelayedCall(delayTime, () =>
                            {
                                // �x�����s������e
                                // �R�}���h�{�^����\������
                                uiManager.ShowCommandButtons();

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
        private void ChangePhase(Phase newPhase)
        {
            // �i�s���[�h��ύX
            nowPhase = newPhase;
        }

        /// <summary>
        /// �I�𒆂̃L�����N�^�[��񏉊�������
        /// </summary>
        private void ClearSelectingChara()
        {
            // �I�𒆂̃L�����N�^�[������������
            selectingCharacter = null;
            // �L�����N�^�[�̃X�e�[�^�X��UI���\���ɂ���
            uiManager.HideStatusWindow();
        }

        /// <summary>
        /// �U������
        /// </summary>
        public void AttackCommand()
        {
            // �R�}���h�{�^�����\���ɂ���
            uiManager.HideCommandButtons();

            // �U���͈͎擾
            // �U���\�ȏꏊ���X�g���擾����
            attackableBlocks = mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            // �U���\�ȏꏊ���X�g��\������
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
        /// �U������
        /// </summary>
        /// <param name="attackChara">�U������L�����N�^�[</param>
        /// <param name="defenseChara">�U�������L�����N�^�[</param>
        private void Attack(Character.Character attackChara, Character.Character defenseChara)
        {
            Debug.Log("�U���� : " + attackChara.characterName
                + "  �h�䑤 : " + defenseChara.characterName);

            // �_���[�W�v�Z
            int damegeValue;    // �_���[�W��
            int attackPoint = attackChara.atk;    // �U�����鑤�̍U����
            int defencePoint = defenseChara.def;   // �U������鑤�̖h���

            // �_���[�W�@���@�U���́@�[�@�h���
            damegeValue = attackPoint - defencePoint;
            // �_���[�W�ʂ�0�ȉ��̎�
            if (damegeValue < 0)
            {
                // 0�ɂ���
                damegeValue = 0;
            }

            // �U���A�j���[�V����
            attackChara.AttackAnimation(defenseChara);

            // �o�g�����ʕ\���E�B���h�E�̕\���ݒ�
            uiManager.battleWindowUI.ShowWindow(defenseChara, damegeValue);

            // �_���[�W�ʕ��U�����ꂽ����HP������������
            defenseChara.nowHP -= damegeValue;
            // HP��0�`�ő�l�͈̔͂Ɏ��܂�悤�ɕ␳
            defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

            // HP��0�ɂȂ����L�����N�^�[���폜����
            if(defenseChara.nowHP == 0)
            {
                characterManager.DeleteCharaData(defenseChara);
            }

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
    }
}