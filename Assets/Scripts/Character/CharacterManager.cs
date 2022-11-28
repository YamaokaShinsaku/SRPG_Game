using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public Transform charactersParent;   // 全キャラクターの親オブジェクトのTransform

        [HideInInspector]
        public List<Character> characters;  // 全キャラクターデータ

        // Start is called before the first frame update
        void Start()
        {
            // マップ上のキャラクター全データを取得
            characters = new List<Character>();
            charactersParent.GetComponentsInChildren(characters);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 指定した座標に存在するキャラクターデータを検索して返す
        /// </summary>
        /// <param name="xPos">x座標</param>
        /// <param name="zPos">z座標</param>
        /// <returns>キャラクターデータ</returns>
        public Character GetCharacterData(int xPos, int zPos)
        {
            // キャラクターデータを検索
            foreach(Character charaData in characters)
            {
                // キャラクターの位置が指定の位置と一致しているかをチェック
                if((charaData.xPos == xPos) && (charaData.zPos == zPos))
                {
                    // データを返す
                    return charaData;
                }
            }

            // データが見つからなければnullを返す
            return null;
        }

        /// <summary>
        /// 指定したキャラクターを削除する
        /// </summary>
        /// <param name="charaData">削除するキャラクターデータ</param>
        public void DeleteCharaData(Character charaData)
        {
            // リストからデータを削除
            characters.Remove(charaData);
            // オブジェクト削除
            Destroy(charaData.gameObject);
        }
    }
}

