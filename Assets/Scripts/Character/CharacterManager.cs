using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public Transform charactersParent;   // �S�L�����N�^�[�̐e�I�u�W�F�N�g��Transform

        [HideInInspector]
        public List<Character> characters;  // �S�L�����N�^�[�f�[�^

        // Start is called before the first frame update
        void Start()
        {
            // �}�b�v��̃L�����N�^�[�S�f�[�^���擾
            characters = new List<Character>();
            charactersParent.GetComponentsInChildren(characters);

            // DataManager����f�[�^�Ǘ��N���X���擾����
            DataManager.Data data =
                GameObject.Find("DataManager").GetComponent<DataManager.Data>();

            // �X�e�[�^�X�㏸�f�[�^��K������
            foreach(Character charaData in characters)
            {
                // �G�L�����N�^�[�͋������Ȃ�
                if(charaData.isEnemy)
                {
                    continue;
                }

                // �L�����N�^�[�̔\�͂��㏸������
                charaData.maxHP += data.addHP;    // �ő�HP
                charaData.nowHP += data.addHP;    // ���݂�HP
                charaData.atk += data.addAtk;     // �U����
                charaData.def += data.addDef;     // �h���
            }
        }

        /// <summary>
        /// �w�肵�����W�ɑ��݂���L�����N�^�[�f�[�^���������ĕԂ�
        /// </summary>
        /// <param name="xPos">x���W</param>
        /// <param name="zPos">z���W</param>
        /// <returns>�L�����N�^�[�f�[�^</returns>
        public Character GetCharacterData(int xPos, int zPos)
        {
            // �L�����N�^�[�f�[�^������
            foreach(Character charaData in characters)
            {
                // �L�����N�^�[�̈ʒu���w��̈ʒu�ƈ�v���Ă��邩���`�F�b�N
                if((charaData.xPos == xPos) && (charaData.zPos == zPos))
                {
                    // �f�[�^��Ԃ�
                    return charaData;
                }
            }

            // �f�[�^��������Ȃ����null��Ԃ�
            return null;
        }

        /// <summary>
        /// �w�肵���L�����N�^�[���폜����
        /// </summary>
        /// <param name="charaData">�폜����L�����N�^�[�f�[�^</param>
        public void DeleteCharaData(Character charaData)
        {
            // ���X�g����f�[�^���폜
            characters.Remove(charaData);
            // �I�u�W�F�N�g�폜
            DOVirtual.DelayedCall(0.5f, () =>
            {
                // �x�����s������e
                //Destroy(charaData.gameObject);
                charaData.gameObject.SetActive(false);
            });

            // �Q�[���I��������s��
            GetComponent<GameManager.GameManager>().CheckFinish();
        }
    }
}

