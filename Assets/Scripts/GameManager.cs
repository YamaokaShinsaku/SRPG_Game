using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager.MapManager mapManager;
        [SerializeField]
        private Character.CharacterManager characterManager;

        // �i�s�Ǘ��p�ϐ�
        private Character.Character selectingCharacter;   // �I�𒆂̃L�����N�^�[
        private List<MapBlock> reachableBlocks;           // �I�𒆂̃L�����N�^�[�̈ړ��\�u���b�N���X�g

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

            // ���X�g��������
            reachableBlocks = new List<MapBlock>();

            // �J�n���̐i�s���[�h
            nowPhase = Phase.MyTurn_Start;
        }

        // Update is called once per frame
        void Update()
        {
            // �^�b�v������o
            if(Input.GetMouseButtonDown(0))
            {
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
                    targetObject.SetSelectionMode(true);

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
                        // �ړ��\�ȏꏊ���X�g���擾����
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // �i�s���[�h��i�߂�
                        ChangePhase(Phase.MyTurn_Moving);
                    }
                    // �L�����N�^�[�����݂��Ȃ��Ƃ�
                    else
                    {
                        Debug.Log("�L�����N�^�[�͑��݂��܂���");
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

                        // �i�s���[�h��i�߂�
                        ChangePhase(Phase.MyTurn_Command);
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
    }

}