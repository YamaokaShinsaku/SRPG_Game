using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class ActiveCharacterUI : MonoBehaviour
    {
        // �L�����N�^�[�̖��O��\������e�L�X�g
        public Text[] text;
        // activeCharacter��List���擾���邽��
        public APBattleManager actionCharactor;

        // Update is called once per frame
        void Update()
        {
            // ���݂̍s���\�L�����N�^�[���X�g�̃T�C�Y���擾
            int nowCharaCount = 0;
            nowCharaCount = actionCharactor.activeCharacters.Count;

            // ���X�g�̃T�C�Y��3�ȏ�̎�
            if (3 <= nowCharaCount )
            {
                text[2].text = actionCharactor.activeCharacters[2].characterName;
                text[1].text = actionCharactor.activeCharacters[1].characterName;
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
            // ���X�g�̃T�C�Y���Q�̎�
            else if (2 == nowCharaCount)
            {
                text[2].text = "None";
                text[1].text = actionCharactor.activeCharacters[1].characterName;
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
            // ���X�g�̃T�C�Y���P�̎�
            else if (1 == nowCharaCount)
            {
                text[2].text = "None";
                text[1].text = "None";
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
        }
    }
}
