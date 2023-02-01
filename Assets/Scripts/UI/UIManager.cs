using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UIManager
{
    public class UIManager : MonoBehaviour
    {
        /// ステータスウィンドウ ///
        public GameObject playerStatusWindow;
        public GameObject enemyStatusWindow;
        public Image hpGageImg;         // HPゲージ画像
        public Image EnemyHpGageImg;    // HPゲージ画像
        // テキスト
        public Text nameText;
        public Text hpText;
        public Text atkText;
        public Text defText;

        public Text EnemyNameText;
        public Text EnemyHpText;
        public Text EnemyAtkText;
        public Text EnemyDefText;

        public Text DirectionText;
        // 属性アイコン
        public Image attributeIcon;
        public Image EnemyAttributeIcon;
        public Sprite Water;    // 水
        public Sprite Fire;     // 火
        public Sprite Wind;     // 風
        public Sprite Soil;     // 土

        // コマンドボタン
        public GameObject commandButtons;       // 全コマンドボタンの親オブジェクト
        public GameObject moveCancelButton;     // 移動キャンセルボタン
        public Button skillCommandButton;       // スキルコマンドのボタン

        // スキル説明用テキスト
        public Text skillText;

        // バトル結果表示ウィンドウ
        public BattleWindowUI battleWindowUI;

        // ロゴ画像
        public Image playerTurnImg;
        public Image enemyTurnImg;
        public Image gameClearImg;
        public Image gameOverImg;

        // フェードイン用画像
        public Image fadeImg;
        Color startAlphaNum;

        // 行動決定・キャンセルボタン
        public GameObject decideButtons;

        // カットイン画像
        public GameObject cutinPanel;
        public RawImage rawImg;

        // 方向選択画像
        public GameObject directionUI;

        // Start is called before the first frame update
        void Start()
        {
            // UI初期化
            HidePlayerStatusWindow();
            HideEnemyStatusWindow();
            HideCommandButtons();
            HideMoveCancelButton();
            HideDecideButtons();
            HideDirectionText();

            // fadeImgのアルファ値の初期設定
            // フェードアウトから開始するため
            startAlphaNum = fadeImg.color;
            startAlphaNum.a = 1.0f;
            fadeImg.color = startAlphaNum;
        }

        public void HideDirectionText()
        {
            DirectionText.enabled = false;
        }
        public void ShowDirectionText()
        {
            DirectionText.enabled = true;
        }

        /// <summary>
        /// キャラクターのステータスを非表示に
        /// </summary>
        /// <param name="character">キャラクター</param>
        public void HideCharaStatus(Character.Character character)
        {
            character.statusUI.SetActive(false);
            character.texture.Release();
        }

        /// <summary>
        /// キャラクターのステータスを表示
        /// </summary>
        /// <param name="character">キャラクター</param>
        public void ShowCharaStatus(Character.Character character)
        {
            character.statusUI.SetActive(true);
        }

        /// <summary>
        /// rawImage に textureを設定する
        /// </summary>
        /// <param name="texture">テクスチャ</param>
        public void SetTexture(Texture texture)
        {
            rawImg.texture = texture;
        }

        public void CutinActive()
        {
            cutinPanel.SetActive(true);
        }

        public void CutinDelete()
        {
            cutinPanel.SetActive(false);
        }

        /// <summary>
        /// ステータスウィンドウを隠す
        /// </summary>
        public void HidePlayerStatusWindow()
        {
            // オブジェクトを非アクティブ化
            playerStatusWindow.SetActive(false);

            // テキストを非表示に
            nameText.enabled = false;
            hpText.enabled = false;
            atkText.enabled = false;
            defText.enabled = false;
        }

        public void HideEnemyStatusWindow()
        {
            // オブジェクトを非アクティブ化
            enemyStatusWindow.SetActive(false);

            // テキストを非表示に
            EnemyNameText.enabled = false;
            EnemyHpText.enabled = false;
            EnemyAtkText.enabled = false;
            EnemyDefText.enabled = false;
        }

        /// <summary>
        /// コマンドボタンを非表示に
        /// </summary>
        public void HideCommandButtons()
        {
            commandButtons.SetActive(false);
            skillCommandButton.gameObject.SetActive(false);

            skillText.enabled = false;
        }

        /// <summary>
        /// 移動キャンセルボタンを非表示に
        /// </summary>
        public void HideMoveCancelButton()
        {
            moveCancelButton.SetActive(false);
        }

        /// <summary>
        /// 行動決定・キャンセルボタンを非表示に
        /// </summary>
        public void HideDecideButtons()
        {
            decideButtons.SetActive(false);
        }

        /// <summary>
        /// ステータスウィンドウを表示する
        /// </summary>
        /// <param name="charaData">キャラクターデータ</param>
        public void ShowPlayerStatusWindow(Character.Character charaData)
        {
            // オブジェクトをアクティブに
            playerStatusWindow.SetActive(true);

            // テキストを表示する
            nameText.enabled = true;
            hpText.enabled = true;
            atkText.enabled = true;
            defText.enabled = true;

            // 名前テキスト表示
            nameText.text = charaData.characterName;

            // 属性画像表示
            switch(charaData.attribute)
            {
                case Character.Character.Attribute.Water:
                    attributeIcon.sprite = Water;
                    break;
                case Character.Character.Attribute.Fire:
                    attributeIcon.sprite = Fire;
                    break;
                case Character.Character.Attribute.Wind:
                    attributeIcon.sprite = Wind;
                    break;
                case Character.Character.Attribute.Soil:
                    attributeIcon.sprite = Soil;
                    break;
            }

            // HPゲージ表示
            // 現在のHP割合をゲージの fillAmount にセットする
            float ratio = (float)charaData.nowHP / charaData.maxHP;
            hpGageImg.fillAmount = ratio;

            // Text表示
            hpText.text = charaData.nowHP + "/" + charaData.maxHP;
            atkText.text = charaData.atk.ToString();
            if(!charaData.isDefBreak)
            {
                defText.text = charaData.def.ToString();
            }
            // 防御力０（デバフ）の時
            else
            {
                defText.text = "<color=red>0</color>";
            }
        }

        public void ShowEnemyStatusWindow(Character.Character charaData)
        {
            // オブジェクトをアクティブに
            enemyStatusWindow.SetActive(true);

            // テキストを表示する
            EnemyNameText.enabled = true;
            EnemyHpText.enabled = true;
            EnemyAtkText.enabled = true;
            EnemyDefText.enabled = true;

            // 名前テキスト表示
            EnemyNameText.text = charaData.characterName;

            // 属性画像表示
            switch (charaData.attribute)
            {
                case Character.Character.Attribute.Water:
                    EnemyAttributeIcon.sprite = Water;
                    break;
                case Character.Character.Attribute.Fire:
                    EnemyAttributeIcon.sprite = Fire;
                    break;
                case Character.Character.Attribute.Wind:
                    EnemyAttributeIcon.sprite = Wind;
                    break;
                case Character.Character.Attribute.Soil:
                    EnemyAttributeIcon.sprite = Soil;
                    break;
            }

            // HPゲージ表示
            // 現在のHP割合をゲージの fillAmount にセットする
            float ratio = (float)charaData.nowHP / charaData.maxHP;
            EnemyHpGageImg.fillAmount = ratio;

            // Text表示
            EnemyHpText.text = charaData.nowHP + "/" + charaData.maxHP;
            EnemyAtkText.text = charaData.atk.ToString();
            if (!charaData.isDefBreak)
            {
                EnemyDefText.text = charaData.def.ToString();
            }
            // 防御力０（デバフ）の時
            else
            {
                EnemyDefText.text = "<color=red>0</color>";
            }
        }

        /// <summary>
        /// コマンドボタンを表示
        /// </summary>
        /// <param name="selectChara">選択中のキャラクター</param>
        public void ShowCommandButtons(Character.Character selectChara)
        {
            commandButtons.SetActive(true);
            skillCommandButton.gameObject.SetActive(true);

            skillText.enabled = true;

            // 選択中のキャラクターのスキルをTextに表示する
            // 選択中のキャラクターのスキル
            Character.SkillDefine.Skill skill = selectChara.skill;
            // スキル名
            string skillName = Character.SkillDefine.dicSkillName[skill];
            // スキルの説明文
            string skillInfo = Character.SkillDefine.dicSkillInfo[skill];

            // リッチテキストでサイズを変更死ながら文字を表示
            skillText.text = "<size=300>" + skillName + "</size>\n" + skillInfo;

            // スキル使用不可能ならスキルボタンを押せないようにする
            if(selectChara.isSkillLock)
            {
                skillCommandButton.interactable = false;
            }
            else
            {
                skillCommandButton.interactable = true;
            }

        }

        /// <summary>
        /// 移動キャンセルボタンを表示
        /// </summary>
        public void ShowMoveCancelButton()
        {
            moveCancelButton.SetActive(true);
        }

        public void ShowDecideButtons()
        {
            decideButtons.SetActive(true);
        }

        /// <summary>
        /// プレイヤーターンのロゴを表示する
        /// </summary>
        public void ShowPlayerTurnLogo()
        {
            // 徐々に表示・非表示を行う
            playerTurnImg.DOFade(
                1.0f,       // 指定数値まで画像のα値を変化させる
                1.0f)       // アニメーション時間
                .SetEase(Ease.OutCubic)             // 変化の度合いを設定
                .SetLoops(2, LoopType.Yoyo);     // ループ回数・方式を設定
        }

        /// <summary>
        /// エネミーターンのロゴを表示する
        /// </summary>
        public void ShowEnemyTurnLogo()
        {
            // 徐々に表示・非表示を行う
            enemyTurnImg.DOFade(
                1.0f,       // 指定数値まで画像のα値を変化させる
                1.0f)       // アニメーション時間
                .SetEase(Ease.OutCubic)             // 変化の度合いを設定
                .SetLoops(2, LoopType.Yoyo);     // ループ回数・方式を設定
        }

        /// <summary>
        /// ゲームクリア画像を表示
        /// </summary>
        public void ShowGameClearLogo()
        {
            // 徐々に表示・非表示を行う
            gameClearImg.DOFade(
                1.0f,       // 指定数値まで画像のα値を変化させる
                1.0f)       // アニメーション時間
                .SetEase(Ease.OutCubic);            // 変化の度合いを設定

            // 拡大・縮小アニメーション
            gameClearImg.transform.DOScale(1.5f, 1.0f)
                .SetEase(Ease.OutCubic)
                .SetLoops(2, LoopType.Yoyo);
        }

        /// <summary>
        /// ゲームオーバー画像を表示
        /// </summary>
        public void ShowGameOverLogo()
        {
            // 徐々に表示・非表示を行う
            gameOverImg.DOFade(
                1.0f,       // 指定数値まで画像のα値を変化させる
                1.0f)       // アニメーション時間
                .SetEase(Ease.OutCubic);            // 変化の度合いを設定
        }

        /// <summary>
        /// フェードイン開始
        /// </summary>
        public void StartFadeIn()
        {
            fadeImg.DOFade(
                1.0f,       // 指定数値まで画像のα値を変化させる
                3.5f)       // アニメーション時間
                .SetEase(Ease.Linear);
        }

        /// <summary>
        /// フェードアウト開始
        /// </summary>
        public void StartFadeOut()
        {
            fadeImg.DOFade(
                0.0f,       // 指定数値まで画像のα値を変化させる
                3.5f)       // アニメーション時間
                .SetEase(Ease.Linear);
        }
    }
}