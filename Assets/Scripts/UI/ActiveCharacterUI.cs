using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class ActiveCharacterUI : MonoBehaviour
    {
        // キャラクターの名前を表示するテキスト
        public Text[] text;
        // activeCharacterのListを取得するため
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
