using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

            // DataManagerからデータ管理クラスを取得する
            DataManager.Data data =
                GameObject.Find("DataManager").GetComponent<DataManager.Data>();

            // ステータス上昇データを適応する
            foreach(Character charaData in characters)
            {
                // 敵キャラクターは強化しない
                if(charaData.isEnemy)
                {
                    continue;
                }

                // キャラクターの能力を上昇させる
                charaData.maxHP += data.addHP;    // 最大HP
                charaData.nowHP += data.addHP;    // 現在のHP
                charaData.atk += data.addAtk;     // 攻撃力
                charaData.def += data.addDef;     // 防御力
            }
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
            DOVirtual.DelayedCall(0.5f, () =>
            {
                // 遅延実行する内容
                //Destroy(charaData.gameObject);
                charaData.gameObject.SetActive(false);
            });

            // ゲーム終了判定を行う
            GetComponent<GameManager.GameManager>().CheckFinish();
        }
    }
}

