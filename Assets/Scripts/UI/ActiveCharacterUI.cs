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
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < actionCharactor.activeCharacters.Count; i++)
            {
                text[i].text = actionCharactor.activeCharacters[i].name;
            }
        }
    }
}
