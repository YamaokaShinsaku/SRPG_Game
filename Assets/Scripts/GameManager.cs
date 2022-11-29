using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager.MapManager mapManager;
        [SerializeField]
        private Character.CharacterManager characterManager;
        [SerializeField]
        private UIManager.UIManager uiManager;

        // 進行管理用変数
        private Character.Character selectingCharacter;   // 選択中のキャラクター
        private List<MapBlock> reachableBlocks;           // 選択中のキャラクターの移動可能ブロックリスト
        private List<MapBlock> attackableBlocks;          // 選択中のキャラクターの攻撃可能ブロックリスト

        // 遅延時間
        const float delayTime = 0.5f;

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
            uiManager = GetComponent<UIManager.UIManager>();

            // リストを初期化
            reachableBlocks = new List<MapBlock>();
            attackableBlocks = new List<MapBlock>();

            // 開始時の進行モード
            nowPhase = Phase.MyTurn_Start;
        }

        // Update is called once per frame
        void Update()
        {
            // タップ先を検出
            if(Input.GetMouseButtonDown(0))
            {
                // デバッグ用処理
                // バトル結果ウィンドウが表示されているとき
                if(uiManager.battleWindowUI.gameObject.activeInHierarchy)
                {
                    // バトル結果ウィンドウを閉じる
                    uiManager.battleWindowUI.HideWindow();

                    // 進行モードを進める
                    ChangePhase(Phase.MyTurn_Start);

                    return;
                }


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
                    targetObject.SetSelectionMode(MapBlock.Highlight.Select);

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
                        // キャラクターのステータスUIを表示する
                        uiManager.ShowStatusWindow(selectingCharacter);
                        // 移動可能な場所リストを取得する
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // 移動可能な場所リストを表示する
                        foreach(MapBlock mapBlock in reachableBlocks)
                        {
                            mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                        }

                        // 進行モードを進める
                        ChangePhase(Phase.MyTurn_Moving);
                    }
                    // キャラクターが存在しないとき
                    else
                    {
                        Debug.Log("キャラクターは存在しません");

                        // 選択中のキャラクター情報を初期化する
                        ClearSelectingChara();
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

                        // 指定時間後に処理を実行する
                        DOVirtual.DelayedCall(delayTime, () =>
                            {
                                // 遅延実行する内容
                                // コマンドボタンを表示する
                                uiManager.ShowCommandButtons();

                                // 進行モードを進める
                                ChangePhase(Phase.MyTurn_Command);
                            });
                    }
                    break;
                // 自分のターン : コマンド選択
                case Phase.MyTurn_Command:
                    // 攻撃処理
                    // 攻撃可能ブロックを選択した場合に攻撃処理を呼ぶ
                    // 攻撃可能ブロックをタップした時
                    if (attackableBlocks.Contains(targetObject))
                    {
                        // 攻撃可能な場所リストを初期化する
                        attackableBlocks.Clear();
                        // 全ブックの選択状態を解除
                        mapManager.AllSelectionModeClear();

                        // 攻撃対象の位置にいるキャラクターデータを取得
                        var targetChara =
                            characterManager.GetCharacterData(targetObject.xPos, targetObject.zPos);
                        // 攻撃対象のキャラクターが存在するとき
                        if (targetChara != null)
                        {
                            // キャラクター攻撃処理
                            Attack(selectingCharacter, targetChara);

                            // 進行モードを進める
                            ChangePhase(Phase.MyTurn_Result);
                            return;
                        }
                        // 攻撃対象が存在しないとき
                        else
                        {
                            // 進行モードを進める
                            ChangePhase(Phase.Enemyturn_Start);
                        }
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

        /// <summary>
        /// 選択中のキャラクター情報初期化する
        /// </summary>
        private void ClearSelectingChara()
        {
            // 選択中のキャラクターを初期化する
            selectingCharacter = null;
            // キャラクターのステータスのUIを非表示にする
            uiManager.HideStatusWindow();
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        public void AttackCommand()
        {
            // コマンドボタンを非表示にする
            uiManager.HideCommandButtons();

            // 攻撃範囲取得
            // 攻撃可能な場所リストを取得する
            attackableBlocks = mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            // 攻撃可能な場所リストを表示する
            foreach(MapBlock mapBlock in attackableBlocks)
            {
                mapBlock.SetSelectionMode(MapBlock.Highlight.Attackable);
            }
        }

        /// <summary>
        /// 待機処理
        /// </summary>
        public void StandCommand()
        {
            // コマンドボタンを非表示に
            uiManager.HideCommandButtons();
            // 進行モードを進める
            ChangePhase(Phase.Enemyturn_Start);
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        /// <param name="attackChara">攻撃するキャラクター</param>
        /// <param name="defenseChara">攻撃されるキャラクター</param>
        private void Attack(Character.Character attackChara, Character.Character defenseChara)
        {
            Debug.Log("攻撃側 : " + attackChara.characterName
                + "  防御側 : " + defenseChara.characterName);

            // ダメージ計算
            int damegeValue;    // ダメージ量
            int attackPoint = attackChara.atk;    // 攻撃する側の攻撃力
            int defencePoint = defenseChara.def;   // 攻撃される側の防御力

            // ダメージ　＝　攻撃力　ー　防御力
            damegeValue = attackPoint - defencePoint;
            // ダメージ量が0以下の時
            if (damegeValue < 0)
            {
                // 0にする
                damegeValue = 0;
            }

            // 攻撃アニメーション
            attackChara.AttackAnimation(defenseChara);

            // バトル結果表示ウィンドウの表示設定
            uiManager.battleWindowUI.ShowWindow(defenseChara, damegeValue);

            // ダメージ量分攻撃された側のHPを減少させる
            defenseChara.nowHP -= damegeValue;
            // HPが0～最大値の範囲に収まるように補正
            defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

            // HPが0になったキャラクターを削除する
            if(defenseChara.nowHP == 0)
            {
                characterManager.DeleteCharaData(defenseChara);
            }

            // ターン切り替え処理
            DOVirtual.DelayedCall(2.0f, () =>
            {
                // 遅延実行する内容
                // ウィンドウを非表示に
                uiManager.battleWindowUI.HideWindow();

                // ターンを切り替える
                if (nowPhase == Phase.MyTurn_Result)
                {
                    // 敵のターンへ
                    ChangePhase(Phase.Enemyturn_Start);
                }
                else if (nowPhase == Phase.EnemyTurn_Result)
                {
                    // 自分のターンへ
                    ChangePhase(Phase.MyTurn_Start);
                }
            });

        }
    }
}