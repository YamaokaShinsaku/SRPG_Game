using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private Character.Character selectingCharacter;       // 選択中のキャラクター
        private List<MapBlock> reachableBlocks;               // 選択中のキャラクターの移動可能ブロックリスト
        private List<MapBlock> attackableBlocks;              // 選択中のキャラクターの攻撃可能ブロックリスト
        private Character.SkillDefine.Skill selectingSkill;   // 選択中のスキル（通常攻撃はNONE固定）

        [SerializeField]
        private bool isFinish;      // ゲーム終了フラグ

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

            // フェードアウト開始
            uiManager.StartFadeOut();

            // フェードアウトが終わったら実行
            DOVirtual.DelayedCall(3.5f, () =>
            {
                // プレイヤーターン開始ロゴ表示
                uiManager.ShowPlayerTurnLogo();
            });
        }

        // Update is called once per frame
        void Update()
        {
            // ゲームが終了しているなら
            if(isFinish)
            {
                return;
            }

            // タップ先を検出
            if(Input.GetMouseButtonDown(0) 
                // UIへのタップを検出する
                && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
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
        /// ゲーム終了条件を満たしているか確認し、満たしていれば終了する
        /// </summary>
        public void CheckFinish()
        {
            // プレイヤー勝利フラグ
            bool isWin = true;
            // プレイヤー敗北フラグ
            bool isLose = true;

            // 生きている味方、敵が存在するかをチェック
            foreach(var charaData in characterManager.characters)
            {
                // 敵がいるとき
                if(charaData.isEnemy)
                {
                    // 勝利フラグをfalse
                    isWin = false;
                }
                // 味方がいるとき
                else
                {
                    // 敗北フラグをfalse
                    isLose = false;
                }
            }

            // 勝利、敗北フラグのどちらかがtrueならゲームを終了
            if(isWin || isLose)
            {
                // ゲーム終了フラグをtrue
                isFinish = true;

                // ロゴ画像を表示
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    // ゲームクリアの時
                    if (isWin)
                    {
                        uiManager.ShowGameClearLogo();
                    }
                    // ゲームオーバーの時
                    else
                    {
                        uiManager.ShowGameOverLogo();
                    }
                    // フェードイン開始
                    uiManager.StartFadeIn();
                });

                // EnhanceSceneの読み込み
                DOVirtual.DelayedCall(7.0f, () =>
                {
                    SceneManager.LoadScene("Enhance");
                });
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
                        uiManager.ShowPlayerStatusWindow(selectingCharacter);
                        // 移動可能な場所リストを取得する
                        reachableBlocks = mapManager.SearchReachableBlocks(charaData.xPos, charaData.zPos);
                        // 移動可能な場所リストを表示する
                        foreach(MapBlock mapBlock in reachableBlocks)
                        {
                            mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                        }

                        // 移動キャンセルボタンを表示
                        uiManager.ShowMoveCancelButton();
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
                    // 敵キャラクターを選択中なら移動をキャンセルして終了
                    if(selectingCharacter.isEnemy)
                    {
                        CancelMoving();
                        break;
                    }

                    // 選択したブロックが移動可能な場所リスト内にあるとき
                    if(reachableBlocks.Contains(targetObject))
                    {
                      // 選択中のキャラクターを移動させる
                        selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);

                        // 移動可能な場所リストを初期化する
                        reachableBlocks.Clear();
                        // 全ブロックの選択状態を解除
                        mapManager.AllSelectionModeClear();
                        // 移動キャンセルボタンを非表示に
                        uiManager.HideMoveCancelButton();

                        // 指定時間後に処理を実行する
                        DOVirtual.DelayedCall(delayTime, () =>
                            {
                                // 遅延実行する内容
                                // コマンドボタンを表示する
                                uiManager.ShowCommandButtons(selectingCharacter);

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
        /// <param name="noLogos">ロゴ非表示フラグ</param>
        private void ChangePhase(Phase newPhase, bool noLogos = false)
        {
            // ゲームが終了しているなら
            if (isFinish)
            {
                return;
            }

            // 進行モードを変更
            nowPhase = newPhase;

            // 特定のモードに切り替わったタイミングで行う処理
            switch (nowPhase)
            {
                // 自分のターン : 開始
                case Phase.MyTurn_Start:
                    if(!noLogos)
                    {
                        // ロゴ画像を表示
                        uiManager.ShowPlayerTurnLogo();
                    }
                    break;
                // 敵のターン : 開始
                case Phase.Enemyturn_Start:
                    if (!noLogos)
                    {
                        // ロゴ画像を表示
                        uiManager.ShowEnemyTurnLogo();
                    }

                    // 敵の行動処理を開始する(遅延実行)
                    DOVirtual.DelayedCall(delayTime, () =>
                    {
                        EnemyCommand();
                    });
                    break;
            }
        }

        /// <summary>
        /// 選択中のキャラクター情報初期化する
        /// </summary>
        private void ClearSelectingChara()
        {
            // 選択中のキャラクターを初期化する
            selectingCharacter = null;
            // キャラクターのステータスのUIを非表示にする
            uiManager.HidePlayerStatusWindow();
        }

        /// <summary>
        /// 攻撃コマンドボタン処理
        /// </summary>
        public void AttackCommand()
        {
            // スキルの選択状態をオフにする
            selectingSkill = Character.SkillDefine.Skill.None;
            // 攻撃範囲を取得して表示
            GetAttackableBlocks();

            //// コマンドボタンを非表示にする
            //uiManager.HideCommandButtons();

            //// 攻撃範囲取得
            //// 攻撃可能な場所リストを取得する
            //attackableBlocks = mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            //// 攻撃可能な場所リストを表示する
            //foreach(MapBlock mapBlock in attackableBlocks)
            //{
            //    mapBlock.SetSelectionMode(MapBlock.Highlight.Attackable);
            //}
        }

        /// <summary>
        /// スキルコマンドボタン処理
        /// </summary>
        public void SkillCommand()
        {
            // キャラクターの持つスキルを選択状態にする
            selectingSkill = selectingCharacter.skill;
            // 攻撃範囲を取得して表示
            GetAttackableBlocks();
        }

        /// <summary>
        /// 攻撃・スキルコマンド選択時に対象ブロックを表示する
        /// </summary>
        private void GetAttackableBlocks()
        {
            // コマンドボタンを非表示にする
            uiManager.HideCommandButtons();
            // 攻撃可能な場所リストを取得する
            // スキル : ファイアボールの場合はマップ全域に対応
            if(selectingSkill == Character.SkillDefine.Skill.FireBall)
            {
                attackableBlocks = mapManager.MapBlocksToList();
            }
            else
            {
                attackableBlocks =
                    mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
            }

            // 攻撃可能な場所リストを表示
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
        /// 選択中のキャラクターの移動入力待ち状態を解除する
        /// </summary>
        public void CancelMoving()
        {
            // 全ブロックの選択状態を解除
            mapManager.AllSelectionModeClear();
            // 移動可能な場所リストを初期化
            reachableBlocks.Clear();
            // 移動キャンセルボタンを非表示に
            uiManager.HideMoveCancelButton();
            // 進行モードを元に戻す
            ChangePhase(Phase.MyTurn_Start, true);
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
            int damageValue;    // ダメージ量
            int attackPoint = attackChara.atk;     // 攻撃する側の攻撃力
            int defencePoint = defenseChara.def;   // 攻撃される側の防御力

            // 防御力０（デバフ）がかかっている時
            if(defenseChara.isDefBreak)
            {
                defencePoint = 0;
            }

            // ダメージ　＝　攻撃力　ー　防御力
            damageValue = attackPoint - defencePoint;
            // 属性相性によるダメージ倍率を計算
            float ratio = GetDamageRatioAttribute(attackChara, defenseChara);       // ダメージ倍率を取得
            damageValue = (int)(damageValue * ratio);       // 倍率を適応(int型に変換)

            // ダメージ量が0以下の時
            if (damageValue < 0)
            {
                // 0にする
                damageValue = 0;
            }

            // 選択したスキルによるダメージ値補正と効果処理
            switch(selectingSkill)
            {
                //クリティカル（会心の一撃）
                case Character.SkillDefine.Skill.Critical:
                    // ダメージ2倍
                    damageValue *= 2;
                    // スキル使用不可能状態に
                    attackChara.isSkillLock = true;
                    break;
                // シールド破壊
                case Character.SkillDefine.Skill.DefBreak:
                    // ダメージ0固定
                    damageValue = 0;
                    // 防御力０（デバフ）をセット
                    defenseChara.isDefBreak = true;
                    break;
                // ヒール
                case Character.SkillDefine.Skill.Heal:
                    // 回復（回復量は攻撃力の半分）
                    damageValue = (int)(attackPoint * -0.5f);
                    break;
                // ファイアボール
                case Character.SkillDefine.Skill.FireBall:
                    // 与えるダメージ半減
                    damageValue /= 2;
                    break;
                // スキル無し or 通常攻撃
                default:
                    break;
            }

            // 攻撃アニメーション
            // ヒール・ファイアボールはアニメーション無し
            if(selectingSkill != Character.SkillDefine.Skill.Heal
                && selectingSkill != Character.SkillDefine.Skill.FireBall)
            {
                attackChara.AttackAnimation(defenseChara);
            }
            // 攻撃が当たったタイミングでSEを再生
            DOVirtual.DelayedCall(0.45f, () =>
                {
                    GetComponent<AudioSource>().Play();
                });

            // バトル結果表示ウィンドウの表示設定
            uiManager.battleWindowUI.ShowWindow(defenseChara, damageValue);

            // ダメージ量分攻撃された側のHPを減少させる
            defenseChara.nowHP -= damageValue;
            // HPが0～最大値の範囲に収まるように補正
            defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

            // HPが0になったキャラクターを削除する
            if(defenseChara.nowHP == 0)
            {
                characterManager.DeleteCharaData(defenseChara);
            }

            // スキルの選択状態を解除する
            selectingSkill = Character.SkillDefine.Skill.None;

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

        /// <summary>
        /// 敵キャラクターのうち一体を行動させてターンを終了する
        /// </summary>
        private void EnemyCommand()
        {
            // スキルの選択状態をオフにする
            selectingSkill = Character.SkillDefine.Skill.None;

            // 生存中の敵キャラクターのリストを作成
            // 敵キャラクターリスト
            var enemyCharacters = new List<Character.Character>();
            foreach(Character.Character charaData in characterManager.characters)
            {
                // 全生存キャラクターから、Enemyフラグ が立っているものをリストに追加
                if(charaData.isEnemy)
                {
                    enemyCharacters.Add(charaData);
                }
            }

            // 攻撃可能なキャラクター・位置の組み合わせを1つランダムに取得
            var actionPlan = TargetFinder.GetRandomactionPlans(mapManager, characterManager, enemyCharacters);
            // 組み合わせのデータが存在すれば、攻撃する
            if(actionPlan != null)
            {
                // 移動処理
                actionPlan.charaData.MovePosition(actionPlan.toMoveBlock.xPos, actionPlan.toMoveBlock.zPos);
                // 攻撃処理(移動後に攻撃開始)
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    Attack(actionPlan.charaData, actionPlan.toAttaackChara);
                });

                // 進行モードを進める
                ChangePhase(Phase.EnemyTurn_Result);
                return;
            }

            // 攻撃可能なキャラクターが見つからなかった時
            // 移動させるキャラクターを一体ランダムに選ぶ
            int randID = Random.Range(0, enemyCharacters.Count);
            //  行動する敵キャラクターのデータを取得
            Character.Character targetEnemy = enemyCharacters[randID];
            // 対象の移動可能な場所リストのなかから１つの場所をランダムに選ぶ
            reachableBlocks =
                mapManager.SearchReachableBlocks(targetEnemy.xPos, targetEnemy.zPos);
            if(reachableBlocks.Count > 0)
            {
                randID = Random.Range(0, reachableBlocks.Count);
                // 移動先のブロックデータ
                MapBlock targetBlock = reachableBlocks[randID];
                // 移動処理
                targetEnemy.MovePosition(targetBlock.xPos, targetBlock.zPos);
            }

            // リストをクリア
            reachableBlocks.Clear();
            attackableBlocks.Clear();

            // 進行モードを進める
            ChangePhase(Phase.MyTurn_Start);
        }

        /// <summary>
        /// 属性相性によるダメージ倍率を返す
        /// </summary>
        /// <param name="attackChara">攻撃するキャラクターデータ</param>
        /// <param name="defenseChara">攻撃されるキャラクターデータ</param>
        /// <returns>ダメージ倍率</returns>
        private float GetDamageRatioAttribute(Character.Character attackChara, Character.Character defenseChara)
        {
            // ダメージ倍率を定義
            const float normal = 1.0f;     // 通常
            const float good = 1.2f;        // 有利
            const float bad = 0.8f;          // 不利

            Character.Character.Attribute attacker = attackChara.attribute;        // 攻撃側の属性
            Character.Character.Attribute defender = defenseChara.attribute;     // 防御側の属性

            // 相性決定処理
            // 属性ごとに有利→不利の順でチェックし、どちらにも当てはまらなければ通常倍率を返す
            switch(attacker)
            {
                // 攻撃側 : 水属性
                case Character.Character.Attribute.Water:
                    if(defender == Character.Character.Attribute.Fire)
                    {
                        return good;
                    }
                    else if( defender == Character.Character.Attribute.Soil)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // 攻撃側 : 火属性
                case Character.Character.Attribute.Fire:
                    if (defender == Character.Character.Attribute.Wind)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Water)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // 攻撃側 : 風属性
                case Character.Character.Attribute.Wind:
                    if (defender == Character.Character.Attribute.Soil)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Fire)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // 攻撃側 : 土属性
                case Character.Character.Attribute.Soil:
                    if (defender == Character.Character.Attribute.Water)
                    {
                        return good;
                    }
                    else if (defender == Character.Character.Attribute.Wind)
                    {
                        return bad;
                    }
                    else
                    {
                        return normal;
                    }
                // デフォルト設定
                default:
                    return normal;
            }
        }
    }
}