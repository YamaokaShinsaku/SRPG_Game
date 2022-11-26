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

        // 進行管理用変数
        private Character.Character selectingCharacter;   // 選択中のキャラクター
        private List<MapBlock> reachableBlocks;           // 選択中のキャラクターの移動可能ブロックリスト

        // ターン進行モード
        private enum Phase
        {
            MyTurn_Start,        // 自分のターン：開始
            MyTurn_Moving,       // 自分のターン：移動先選択
            MyTurn_Command,      // 自分のターン：コマンド選択
            MyTurn_Targeting,    // 自分のターン：攻撃対象選択
            MyTurn_Result,       // 自分のターン：結果表示
            Enemyturn_Start,     // 敵のターン  ：開始
            EnemyTurn_Result     // 敵のターン  ：結果表示
        }
        private Phase nowPhase;     // 現在の進行モード

        // Start is called before the first frame update
        void Start()
        {
            mapManager = GetComponent<MapManager.MapManager>();
            characterManager = GetComponent<Character.CharacterManager>();

            // リストを初期化
            reachableBlocks = new List<MapBlock>();

            // 開始時の進行モード
            nowPhase = Phase.MyTurn_Start;
        }

        // Update is called once per frame
        void Update()
        {
            // タップ先を検出
            if(Input.GetMouseButtonDown(0))
            {
                GetMapObjects();
            }
        }

        /// <summary>
        /// タップした場所のオブジェクトを取得
        /// </summary>
        private void GetMapObjects()
        {
            GameObject targetObject = null;     // タップしたオブジェクト

            // タップした方向にカメラからRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            // Rayの当たる位置に存在するオブジェクトを取得
            if(Physics.Raycast(ray, out hit))
            {
                targetObject = hit.collider.gameObject;
            }

            // 対象オブジェクトが存在する場合の処理
            if(targetObject != null)
            {
                SelectBlock(targetObject.GetComponent<MapBlock>());
            }
        }

        /// <summary>
        /// 指定したブロックを選択状態にする
        /// </summary>
        /// <param name="targetBlock">対象のオブジェクトデータ</param>
        private void SelectBlock(MapBlock targetObject)
        {
            // 進行モードごとに処理を分岐する
            switch(nowPhase)
            {
                // 自分のターン ： 開始
                case Phase.MyTurn_Start:
                    // 全ブロックの選択状態を解除する
                    mapManager.AllSelectionModeClear();
                    // ブロックを選択状態にする
                    targetObject.SetSelectionMode(true);

                    Debug.Log("オブジェクトがタップされました \nブロック座標 : "
                        + targetObject.transform.position);

                    // 選択した位置にいるキャラクターデータを取得
                    var charaData =
                        characterManager.GetCharacterData(targetObject.xPos, targetObject.zPos);
                    // キャラクターが存在するとき
                    if (charaData != null)
                    {
                        Debug.Log("キャラクターが存在します : "
                            + charaData.gameObject.name);

                        // 選択中のキャラクター情報に記憶する
                        selectingCharacter = charaData;
                        // 移動可能な場所リストを取得する
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // 進行モードを進める
                        ChangePhase(Phase.MyTurn_Moving);
                    }
                    // キャラクターが存在しないとき
                    else
                    {
                        Debug.Log("キャラクターは存在しません");
                    }
                    break;
                // 自分のターン : 移動
                case Phase.MyTurn_Moving:
                    // 選択したブロックが移動可能な場所リスト内にあるとき
                    if(reachableBlocks.Contains(targetObject))
                    {
                      // 選択中のキャラクターを移動させる
                        selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);

                        // 移動可能な場所リストを初期化する
                        reachableBlocks.Clear();
                        // 全ブロックの選択状態を解除
                        mapManager.AllSelectionModeClear();

                        // 進行モードを進める
                        ChangePhase(Phase.MyTurn_Command);
                    }
                    break;
            }


        }

        /// <summary>
        /// ターン進行モードを変更
        /// </summary>
        /// <param name="newPhase">変更先のモード</param>
        private void ChangePhase(Phase newPhase)
        {
            // 進行モードを変更
            nowPhase = newPhase;
        }
    }

}