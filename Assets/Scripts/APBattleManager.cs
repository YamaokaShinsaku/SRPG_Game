using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class APBattleManager : MonoBehaviour
{
    [SerializeField]
    private MapManager.MapManager mapManager;
    [SerializeField]
    private Character.CharacterManager characterManager;
    [SerializeField]
    private UIManager.UIManager uiManager;

    // 進行管理用変数
    public Character.Character selectingCharacter;       // 選択中のキャラクター
    private Character.SkillDefine.Skill selectingSkill;   // 選択中のスキル（通常攻撃はNONE固定）
    private List<MapBlock> reachableBlocks;               // 選択中のキャラクターの移動可能ブロックリスト
    private List<MapBlock> attackableBlocks;              // 選択中のキャラクターの攻撃可能ブロックリスト

    public List<Character.Character> activeCharacters;     // isActiveがtrueになっているキャラクターのリスト
    public List<Character.Character> enemyList;            // isEnemyがtrueになっているキャラクターのリスト

    // 行動キャンセル処理用変数
    private MapBlock charaAttackBlock;        // 選択キャラクターの攻撃先のブロック
    private int charaStartPositionX;          // 選択キャラクターのX座標
    private int charaStartPositionZ;          // 選択キャラクターのZ座標

    [SerializeField]
    private bool isFinish;      // ゲーム終了フラグ

    // 遅延時間
    const float delayTime = 0.5f;

    // ターン進行モード
    private enum Phase
    {
        C_Start,             // アクティブキャラクター選択フェーズ
        C_SelectDirection,   // キャラクターの向きを選択
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
        // コンポーネントを取得
        mapManager = GetComponent<MapManager.MapManager>();
        characterManager = GetComponent<Character.CharacterManager>();
        uiManager = GetComponent<UIManager.UIManager>();

        // リストを初期化
        reachableBlocks = new List<MapBlock>();
        attackableBlocks = new List<MapBlock>();
        activeCharacters = new List<Character.Character>();

        // 開始時の進行モード
        nowPhase = Phase.C_Start;

        // フェードアウト開始
        uiManager.StartFadeOut();

        // 最初のキャラクターをセット
        SetFirstActionCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームが終了しているなら
        if (isFinish)
        {
            return;
        }

        // 向き選択フェーズのとき
        if(nowPhase == Phase.C_SelectDirection)
        {
            // 矢印キーの方向を向く
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Forward;
                ChangePhase(Phase.C_Start);
                // オブジェクトを取得
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Backward;
                ChangePhase(Phase.C_Start);
                // オブジェクトを取得
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Left;
                ChangePhase(Phase.C_Start);
                // オブジェクトを取得
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Right;
                ChangePhase(Phase.C_Start);
                // オブジェクトを取得
                GetMapObjects();
            }
        }

        // タップ先を検出
        if (Input.GetMouseButtonDown(0)
            // UIへのタップを検出する
            && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            // デバッグ用処理
            // バトル結果ウィンドウが表示されているとき
            if (uiManager.battleWindowUI.gameObject.activeInHierarchy)
            {
                // バトル結果ウィンドウを閉じる
                uiManager.battleWindowUI.HideWindow();

                // 進行モードを進める
                ChangePhase(Phase.C_Start);

                return;
            }
            // オブジェクトを取得
            GetMapObjects();
        }
    }

    /// <summary>
    /// 選択したキャラクターのステータスを表示
    /// </summary>
    /// <param name="targetObject"></param>
    public void OpenStatus(MapBlock targetObject)
    {
        // キャラクターデータを検索
        foreach (Character.Character charaData in characterManager.characters)
        {
            // キャラクターの位置が指定の位置と一致しているかをチェック
            if ((charaData.xPos == targetObject.xPos) && (charaData.zPos == targetObject.zPos))
            {
                // タップした座標にいるキャラクターのUIを表示
                charaData.image.texture = charaData.texture;
                uiManager.ShowCharaStatus(charaData);
                if (charaData.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(charaData);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(charaData);
                }
            }
        }
    }

    /// <summary>
    /// isActiveがtrueになっているキャラクターを取得
    /// </summary>
    public void SetFirstActionCharacter()
    {
        // isActiveがtrueなキャラクターのリストを作成
        if (activeCharacters.Count == 0)
        {
            //activeCharacters = new List<Character.Character>();
            foreach (Character.Character activeCharaData in characterManager.characters)
            {
                // 全生存キャラクターから、isActiveフラグがtrueのキャラクターをリストに追加
                if (activeCharaData.isActive)
                {
                    activeCharacters.Add(activeCharaData);
                }
            }
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
        foreach (var charaData in characterManager.characters)
        {
            // 敵がいるとき
            if (charaData.isEnemy)
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
        if (isWin || isLose)
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
        if (Physics.Raycast(ray, out hit))
        {
            targetObject = hit.collider.gameObject;
        }

        // 対象オブジェクトが存在する場合の処理
        if (targetObject != null)
        {
            SelectBlock(targetObject.GetComponent<MapBlock>());
            OpenStatus(targetObject.GetComponent<MapBlock>());
        }
    }

    /// <summary>
    /// 指定したブロックを選択状態にする
    /// </summary>
    /// <param name="targetBlock">対象のオブジェクトデータ</param>
    private void SelectBlock(MapBlock targetObject)
    {
        // 進行モードごとに処理を分岐する
        switch (nowPhase)
        {
            // 行動するキャラクターがエネミーかどうかを判定
            case Phase.C_Start:
                mapManager.AllSelectionModeClear();
                uiManager.CutinDelete();
                uiManager.HideDirectionText();
                // isActiveがtrueなキャラクターのリストを作成
                foreach (Character.Character activeCharaData in characterManager.characters)
                {
                    activeCharaData.activePoint++;
                    if (activeCharaData.activePoint >= 3)
                    {
                        activeCharaData.isActive = true;
                        activeCharaData.activePoint = 3;
                    }

                    if(activeCharaData.activePoint < 3)
                    {
                        activeCharaData.activePoint += 3;
                    }

                    // 全生存キャラクターから、isActiveフラグがtrueのキャラクターをリストに追加
                    if (activeCharaData.isActive)
                    {
                        if(!activeCharacters.Contains(activeCharaData))
                        {
                            activeCharacters.Add(activeCharaData);
                        }
                    }
                }

                // activeCharactersの先頭のキャラクターを選択中のキャラクター情報に記憶
                selectingCharacter = activeCharacters[0];
                // 選択中のキャラクターの現在位置を記憶
                charaStartPositionX = selectingCharacter.xPos;
                charaStartPositionZ = selectingCharacter.zPos;

                // キャラクターのステータスUIを表示する
                //selectingCharacter.statusUI.SetActive(true);
                selectingCharacter.image.texture = selectingCharacter.texture;
                uiManager.SetTexture(selectingCharacter.texture);
                uiManager.ShowCharaStatus(selectingCharacter);
                selectingCharacter.selectingObj.SetActive(true);
                if(selectingCharacter.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(selectingCharacter);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(selectingCharacter);
                }
                // 移動可能な場所リストを取得する
                reachableBlocks =
                    mapManager.SearchReachableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
                // 移動可能な場所リストを表示する
                foreach (MapBlock mapBlock in reachableBlocks)
                {
                    mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                }

                // 選択したキャラクターがエネミーの時
                if (selectingCharacter.isEnemy)
                {
                    if(!enemyList.Contains(selectingCharacter))
                    {
                        enemyList.Add(selectingCharacter);
                    }
                    ChangePhase(Phase.Enemyturn_Start);
                }
                // プレイヤーキャラの時
                else
                {
                    ChangePhase(Phase.MyTurn_Start);
                }

                break;
            // 自分のターン ： 開始
            case Phase.MyTurn_Start:
                // 全ブロックの選択状態を解除する
                mapManager.AllSelectionModeClear();
                // ブロックを選択状態にする
                targetObject.SetSelectionMode(MapBlock.Highlight.Select);

                Debug.Log("オブジェクトがタップされました \nブロック座標 : "
                    + targetObject.transform.position);

                // キャラクターのステータスUIを表示する
                if (selectingCharacter.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(selectingCharacter);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(selectingCharacter);
                }

                // 移動可能な場所リストを取得する
                reachableBlocks =
                    mapManager.SearchReachableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
                // 移動可能な場所リストを表示する
                foreach (MapBlock mapBlock in reachableBlocks)
                {
                    mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                }

                // 移動キャンセルボタンを表示
                uiManager.ShowMoveCancelButton();
                // 進行モードを進める
                ChangePhase(Phase.MyTurn_Moving); 
 
                break;
            // 自分のターン : 移動
            case Phase.MyTurn_Moving:
                // 敵キャラクターを選択中なら移動をキャンセルして終了
                if (selectingCharacter.isEnemy)
                {
                    CancelMoving();
                    break;
                }

                // 選択したブロックが移動可能な場所リスト内にあるとき
                if (reachableBlocks.Contains(targetObject))
                {
                    // 選択中のキャラクターを移動させる
                    selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);
                    selectingCharacter.animation.SetBool("WalkFlag", true);

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
                        selectingCharacter.animation.SetBool("WalkFlag", false);

                        // 進行モードを進める
                        ChangePhase(Phase.MyTurn_Command);
                    });
                }
                break;
            // 自分のターン : コマンド選択
            case Phase.MyTurn_Command:
                // 攻撃範囲のブロックを選択したとき、行動するかどうかのボタンを表示
                if (attackableBlocks.Contains(targetObject))
                {
                    // 攻撃先のブロック情報を記憶
                    charaAttackBlock = targetObject;
                    // 行動決定・キャンセルボタンを表示する
                    uiManager.ShowDecideButtons();

                    // 攻撃可能な場所リストを初期化
                    attackableBlocks.Clear();
                    // 全ブロックの選択状態を解除
                    mapManager.AllSelectionModeClear();

                    // 攻撃先のブロックを強調表示
                    charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Attackable);

                    // 攻撃対象の位置にいるキャラクターのデータを取得
                    var targetChara =
                        characterManager.GetCharacterData(charaAttackBlock.xPos, charaAttackBlock.zPos);

                    if(targetChara != null)
                    {
                        //targetChara.statusUI.SetActive(true);
                        uiManager.ShowCharaStatus(targetChara);

                    }

                    // 進行モードを進める
                    ChangePhase(Phase.MyTurn_Targeting);
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
                if (!noLogos)
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
            case Phase.C_SelectDirection:
                uiManager.ShowDirectionText();
                Debug.Log("SelectDirectionPhase");
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
        uiManager.HideEnemyStatusWindow();
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
        if (selectingSkill == Character.SkillDefine.Skill.FireBall)
        {
            attackableBlocks = mapManager.MapBlocksToList();
        }
        else
        {
            attackableBlocks =
                mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
        }

        // 攻撃可能な場所リストを表示
        foreach (MapBlock mapBlock in attackableBlocks)
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

        //selectingCharacter.statusUI.SetActive(false);
        uiManager.HideCharaStatus(selectingCharacter);
        selectingCharacter.texture.Release();
        selectingCharacter.selectingObj.SetActive(false);
        uiManager.HidePlayerStatusWindow();
        uiManager.HideEnemyStatusWindow();
        // 選択中のキャラクターをリストから削除
        selectingCharacter.isActive = false;
        activeCharacters.RemoveAt(0);

        selectingCharacter.activePoint--;
        foreach (Character.Character charaData in characterManager.characters)
        {
            // 全生存キャラクターのactivePointを加算
            charaData.activePoint++;
        }

        // 進行モードを進める
        //ChangePhase(Phase.C_Start);
        ChangePhase(Phase.C_SelectDirection);
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
    /// 攻撃時のキャラクターの向きを修正
    /// </summary>
    /// <param name="attackChara">攻撃するキャラクター</param>
    /// <param name="defenseChara">攻撃されるキャラクター</param>
    private void ChangeAttackDirection(Character.Character attackChara, Character.Character defenseChara)
    {
        Character.Character.Direction attacker = attackChara.direction;        // 攻撃側の向き
        Character.Character.Direction defender = defenseChara.direction;       // 防御側の向き

        // 上側のキャラクターを攻撃するとき
        if (attackChara.zPos < defenseChara.zPos
            &&( attackChara.xPos + 1 == defenseChara.xPos
            || attackChara.xPos == defenseChara.xPos
            || attackChara.xPos - 1 == defenseChara.xPos))
        {
            Debug.Log("Forw :" + attackChara.zPos + " : " + defenseChara.zPos);
            attackChara.direction = Character.Character.Direction.Forward;
        }
        // 下側のキャラクターを攻撃するとき
        if (attackChara.zPos > defenseChara.zPos
            &&( attackChara.xPos + 1 == defenseChara.xPos
            || attackChara.xPos == defenseChara.xPos
            || attackChara.xPos - 1 == defenseChara.xPos))
        {
            Debug.Log("Back :" + attackChara.zPos + " : " + defenseChara.zPos);
            attackChara.direction = Character.Character.Direction.Backward;
        }

        if (attacker != Character.Character.Direction.Right)
        {
            // 右側のキャラクターを攻撃するとき
            if (attackChara.xPos + 1 == defenseChara.xPos
                && attackChara.zPos == defenseChara.zPos)
            {
                attackChara.direction = Character.Character.Direction.Right;
            }
        }
        if (attacker != Character.Character.Direction.Left)
        {
            // 左側のキャラクターを攻撃するとき
            if (attackChara.xPos - 1 == defenseChara.xPos
                && attackChara.zPos == defenseChara.zPos)
            {
                attackChara.direction = Character.Character.Direction.Left;
            }
        }
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

        //attackChara.statusUI.SetActive(true);
        //defenseChara.statusUI.SetActive(true);
        attackChara.image.texture = attackChara.texture;
        defenseChara.image.texture = defenseChara.texture;
        uiManager.rawImg.texture = attackChara.texture;
        uiManager.ShowCharaStatus(attackChara);
        uiManager.ShowCharaStatus(defenseChara);

        if (attackChara.isEnemy)
        {
            uiManager.ShowPlayerStatusWindow(defenseChara);
            uiManager.ShowEnemyStatusWindow(attackChara);
        }
        else
        {
            uiManager.ShowPlayerStatusWindow(attackChara);
            uiManager.ShowEnemyStatusWindow(defenseChara);
        }


        // ダメージ計算
        int damageValue;    // ダメージ量
        int attackPoint = attackChara.atk;     // 攻撃する側の攻撃力
        int defencePoint = defenseChara.def;   // 攻撃される側の防御力

        // 防御力０（デバフ）がかかっている時
        if (defenseChara.isDefBreak)
        {
            defencePoint = 0;
        }

        // ダメージ　＝　攻撃力　ー　防御力
        damageValue = attackPoint - defencePoint;
        // 属性相性によるダメージ倍率を計算
        // ダメージ倍率を取得
        float ratio =
            GetDamageRatioAttribute(attackChara, defenseChara)
            + GetDamageRatioDirection(attackChara, defenseChara);    // バックアタック含む
        //float ratio =
        //   GetDamageRatioAttribute(attackChara, defenseChara);    // 属性のみ
        damageValue = (int)(damageValue * ratio);       // 倍率を適応(int型に変換)

        // ダメージ量が0以下の時
        if (damageValue < 0)
        {
            // 0にする
            damageValue = 0;
        }

        // 選択したスキルによるダメージ値補正と効果処理
        switch (selectingSkill)
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
        if (selectingSkill != Character.SkillDefine.Skill.Heal
            && selectingSkill != Character.SkillDefine.Skill.FireBall)
        {
            //uiManager.CutinActive();
            attackChara.AttackAnimation(defenseChara);
            attackChara.animation.SetBool("AttackFlag", true);
            defenseChara.animation.SetBool("DamageFlag", true);
        }
        // 攻撃が当たったタイミングでSEを再生
        DOVirtual.DelayedCall(0.45f, () =>
        {
            GetComponent<AudioSource>().Play();
            attackChara.animation.SetBool("AttackFlag", false);
            defenseChara.animation.SetBool("DamageFlag", false);
        });

        // バトル結果表示ウィンドウの表示設定
        uiManager.battleWindowUI.ShowWindow(defenseChara, damageValue);

        // ダメージ量分攻撃された側のHPを減少させる
        defenseChara.nowHP -= damageValue;
        // HPが0～最大値の範囲に収まるように補正
        defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

        // HPが0になったキャラクターを削除する
        if (defenseChara.nowHP == 0)
        {
            characterManager.DeleteCharaData(defenseChara);
            activeCharacters.Remove(defenseChara);
        }

        // スキルの選択状態を解除する
        selectingSkill = Character.SkillDefine.Skill.None;

        // ターン切り替え処理
        DOVirtual.DelayedCall(2.0f, () =>
        {
            // 遅延実行する内容
            // ウィンドウを非表示に
            uiManager.battleWindowUI.HideWindow();
            uiManager.HideCharaStatus(attackChara);
            uiManager.HideCharaStatus(defenseChara);
            // ターンを切り替える
            if (nowPhase == Phase.MyTurn_Result)
            {
                DOVirtual.DelayedCall(1.0f, () =>
                {
                    //attackChara.statusUI.SetActive(false);
                    //defenseChara.statusUI.SetActive(false);
                    //uiManager.CutinDelete();
                    attackChara.texture.Release();
                    defenseChara.texture.Release();
                    uiManager.HidePlayerStatusWindow();
                    uiManager.HideEnemyStatusWindow();
                });
                attackChara.texture.Release();
                defenseChara.texture.Release();
                selectingCharacter.selectingObj.SetActive(false);
                // 選択中のキャラクターをリストから削除
                selectingCharacter.isActive = false;
                activeCharacters.RemoveAt(0);

                selectingCharacter.activePoint -= 2;

                // キャラ選択フェーズに
                //ChangePhase(Phase.C_Start);
                ChangePhase(Phase.C_SelectDirection);
            }
            else if (nowPhase == Phase.EnemyTurn_Result)
            {
                attackChara.texture.Release();
                defenseChara.texture.Release();
                uiManager.HidePlayerStatusWindow();
                uiManager.HideEnemyStatusWindow();
                attackChara.texture.Release();
                defenseChara.texture.Release();
                selectingCharacter.selectingObj.SetActive(false);
                // 選択中のキャラクターをリストから削除
                selectingCharacter.isActive = false;
                activeCharacters.RemoveAt(0);
                if(selectingCharacter.isEnemy)
                {
                    enemyList.RemoveAt(0);
                }
                selectingCharacter.activePoint -= 2;

                // キャラ選択フェーズに
                ChangePhase(Phase.C_Start);
                //ChangePhase(Phase.C_SelectDirection);
            }
        });

    }

    /// <summary>
    /// 行動内容決定処理
    /// </summary>
    public void ActionDecide()
    {
        // 行動決定・キャンセルボタンを非表示に
        uiManager.HideDecideButtons();
        // 攻撃先のブロックの強調表示を解除する
        charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Off);

        // 攻撃対象の位置にいるキャラクターのデータを取得
        var targetChara =
            characterManager.GetCharacterData(charaAttackBlock.xPos, charaAttackBlock.zPos);

        // 攻撃対象のキャラクターが存在するとき
        if(targetChara != null)
        {
            // キャラクター攻撃処理
            Attack(selectingCharacter, targetChara);
            ChangeAttackDirection(selectingCharacter, targetChara);
            //targetChara.statusUI.SetActive(false);
            //selectingCharacter.statusUI.SetActive(false);
            //uiManager.HideCharaStatus(targetChara);
            //uiManager.HideCharaStatus(selectingCharacter);
            targetChara.texture.Release();
            selectingCharacter.texture.Release();
            selectingCharacter.selectingObj.SetActive(false);
            uiManager.HidePlayerStatusWindow();
            uiManager.HideEnemyStatusWindow();
            // 選択中のキャラクターをリストから削除
            selectingCharacter.isActive = false;
            activeCharacters.RemoveAt(0);

            foreach (Character.Character charaData in characterManager.characters)
            {
                // 全生存キャラクターのactivePointを加算
                charaData.activePoint++;
            }
            // 進行モードを進める
            ChangePhase(Phase.MyTurn_Result);
        }
        // 攻撃対象が存在しない
        else
        {
            // 選択中のキャラクターをリストから削除
            selectingCharacter.selectingObj.SetActive(false);
            uiManager.HidePlayerStatusWindow();
            uiManager.HideEnemyStatusWindow();
            targetChara.texture.Release();
            selectingCharacter.texture.Release();
            selectingCharacter.isActive = false;
            activeCharacters.RemoveAt(0);
            selectingCharacter.activePoint--;
            foreach (Character.Character charaData in characterManager.characters)
            {
                // 全生存キャラクターのactivePointを加算
                charaData.activePoint++;
            }

            // 進行モードを進める
            //ChangePhase(Phase.C_Start);
            ChangePhase(Phase.C_SelectDirection);
        }
    }

    /// <summary>
    /// 行動内容リセット処理
    /// </summary>
    public void ActionCancel()
    {
        // 行動決定・キャンセルボタンを非表示に
        uiManager.HideDecideButtons();
        // 攻撃先のブロックの強調表示を解除
        charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Off);

        // キャラクターを移動前の位置に戻す
        selectingCharacter.MovePosition(charaStartPositionX, charaStartPositionZ);
        // キャラクターの選択状態を解除する
        //ClearSelectingChara();

        // 進行モードを戻す
        ChangePhase(Phase.MyTurn_Start, true);
    }

    /// <summary>
    /// 敵キャラクターのうち一体を行動させてターンを終了する
    /// </summary>
    private void EnemyCommand()
    {
        // スキルの選択状態をオフにする
        selectingSkill = Character.SkillDefine.Skill.None;

        //selectingCharacter.statusUI.SetActive(true);
        uiManager.ShowCharaStatus(selectingCharacter);

        // 攻撃可能なキャラクター・位置の組み合わせを1つランダムに取得
        var actionPlan = TargetFinder.GetRandomactionPlans(mapManager, characterManager, enemyList);
        // 組み合わせのデータが存在すれば、攻撃する
        if (actionPlan != null)
        {
            // 移動処理
            actionPlan.charaData.MovePosition(actionPlan.toMoveBlock.xPos, actionPlan.toMoveBlock.zPos);
            //actionPlan.charaData.animation.SetBool("WalkFlag", true);
            // 攻撃処理(移動後に攻撃開始)
            DOVirtual.DelayedCall(delayTime, () =>
            {
                //actionPlan.charaData.animation.SetBool("WalkFlag", false);
                mapManager.AllSelectionModeClear();
                Attack(actionPlan.charaData, actionPlan.toAttaackChara);
                ChangeAttackDirection(actionPlan.charaData, actionPlan.toAttaackChara);
            });

            // 進行モードを進める
            ChangePhase(Phase.EnemyTurn_Result);
            return;
        }

        // 攻撃可能なキャラクターが見つからなかった時
        // 移動させるキャラクターを一体ランダムに選ぶ
        int randID = Random.Range(0, enemyList.Count);
        //  行動する敵キャラクターのデータを取得
        Character.Character targetEnemy = enemyList[0];
        // 対象の移動可能な場所リストのなかから１つの場所をランダムに選ぶ
        reachableBlocks =
            mapManager.SearchReachableBlocks(targetEnemy.xPos, targetEnemy.zPos);
        if (reachableBlocks.Count > 0)
        {
            randID = Random.Range(0, reachableBlocks.Count);
            // 移動先のブロックデータ
            MapBlock targetBlock = reachableBlocks[randID];
            // 移動処理
            //targetEnemy.animation.SetBool("AttackFlag", true);
            targetEnemy.MovePosition(targetBlock.xPos, targetBlock.zPos);
            //DOVirtual.DelayedCall(delayTime, () =>
            //{
            //    targetEnemy.animation.SetBool("AttackFlag", false);
            //});
        }

        // リストをクリア
        reachableBlocks.Clear();
        attackableBlocks.Clear();

        mapManager.AllSelectionModeClear();

        // 選択中のキャラクターをリストから削除
        selectingCharacter.selectingObj.SetActive(false);
        //selectingCharacter.statusUI.SetActive(false);
        //targetEnemy.statusUI.SetActive(false);
        //uiManager.HideCharaStatus(selectingCharacter);
        //uiManager.HideCharaStatus(targetEnemy);
        uiManager.HidePlayerStatusWindow();
        uiManager.HideEnemyStatusWindow();
        selectingCharacter.isActive = false;
        activeCharacters.RemoveAt(0);
        enemyList.RemoveAt(0);
        selectingCharacter.activePoint--;
        foreach (Character.Character charaData in characterManager.characters)
        {
            // 全生存キャラクターのactivePointを加算
            charaData.activePoint++;
        }

        DOVirtual.DelayedCall(delayTime, () =>
        {
            // 進行モードを進める
            ChangePhase(Phase.C_Start);
        });
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
        const float good = 1.2f;       // 有利
        const float bad = 0.8f;        // 不利

        Character.Character.Attribute attacker = attackChara.attribute;        // 攻撃側の属性
        Character.Character.Attribute defender = defenseChara.attribute;       // 防御側の属性

        // 相性決定処理
        // 属性ごとに有利→不利の順でチェックし、どちらにも当てはまらなければ通常倍率を返す
        switch (attacker)
        {
            // 攻撃側 : 水属性
            case Character.Character.Attribute.Water:
                if (defender == Character.Character.Attribute.Fire)
                {
                    return good;
                }
                else if (defender == Character.Character.Attribute.Soil)
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

    /// <summary>
    /// キャラクターの向きに応じたダメージを返す
    /// </summary>
    /// <param name="attackChara">攻撃するキャラクターデータ</param>
    /// <param name="defenseChara">攻撃されるキャラクターデータ</param>
    /// <returns>ダメージ倍率</returns>
    private float GetDamageRatioDirection(Character.Character attackChara, Character.Character defenseChara)
    {
        // ダメージ倍率を定義
        //const float normal = 1.0f;      // 通常
        const float good = 5.8f;        // バックアタック(0.8倍)

        Character.Character.Direction attacker = attackChara.direction;        // 攻撃側の向き
        Character.Character.Direction defender = defenseChara.direction;       // 防御側の向き

        // 相性決定処理
        // 向きごとに有利→不利の順でチェックし、バックアタックでなければnormalを返す
        switch (defender)
        {
            // 防御側 : Forward
            case Character.Character.Direction.Forward:
                if(attackChara.zPos == defenseChara.zPos - 1
                    && attackChara.xPos == defenseChara.xPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // 防御側 : Backward
            case Character.Character.Direction.Backward:
                if(attackChara.zPos == defenseChara.zPos + 1
                    && attackChara.xPos == defenseChara.xPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // 防御側 : Right
            case Character.Character.Direction.Right:
                if(attackChara.xPos == defenseChara.xPos - 1
                    && attackChara.zPos == defenseChara.zPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // 防御側 : Left
            case Character.Character.Direction.Left:
                if (attackChara.xPos == defenseChara.xPos + 1
                    && attackChara.zPos == defenseChara.zPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // デフォルト設定
            default:
                return 0.0f;
        }
    }

    /// <summary>
    /// 選択中のキャラクターの向きを変更
    /// </summary>
    public void DirectionForward()
    {
        selectingCharacter.direction = Character.Character.Direction.Forward;
    }
    public void DirectionBackward()
    {
        selectingCharacter.direction = Character.Character.Direction.Backward;
    }
    public void DirectionRight()
    {
        selectingCharacter.direction = Character.Character.Direction.Right;
    }
    public void DirectionLeft()
    {
        selectingCharacter.direction = Character.Character.Direction.Left;
    }
}