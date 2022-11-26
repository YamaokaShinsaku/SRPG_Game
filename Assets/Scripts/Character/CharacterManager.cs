using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        // Update is called once per frame
        void Update()
        {

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
    }
}
