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

        // Update is called once per frame
        void Update()
        {
            // 現在の行動可能キャラクターリストのサイズを取得
            int nowCharaCount = 0;
            nowCharaCount = actionCharactor.activeCharacters.Count;

            // リストのサイズが3以上の時
            if (3 <= nowCharaCount )
            {
                text[2].text = actionCharactor.activeCharacters[2].characterName;
                text[1].text = actionCharactor.activeCharacters[1].characterName;
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
            // リストのサイズが２の時
            else if (2 == nowCharaCount)
            {
                text[2].text = "None";
                text[1].text = actionCharactor.activeCharacters[1].characterName;
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
            // リストのサイズが１の時
            else if (1 == nowCharaCount)
            {
                text[2].text = "None";
                text[1].text = "None";
                text[0].text = actionCharactor.activeCharacters[0].characterName;
            }
        }
    }
}
