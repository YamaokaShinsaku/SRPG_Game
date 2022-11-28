using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class UIManager : MonoBehaviour
    {
        /// ステータスウィンドウ ///
        public GameObject statusWindow;
        public Image hpGageImg;         // HPゲージ画像
                                        // テキスト
        public Text nameText;
        public Text hpText;
        public Text atkText;
        public Text defText;
        // 属性アイコン
        public Image attributeIcon;
        public Sprite Water;    // 水
        public Sprite Fire;     // 火
        public Sprite Wind;     // 風
        public Sprite Soil;     // 土

        // コマンドボタン
        public GameObject commandButtons;   // 全コマンドボタンの親オブジェクト

        public BattleWindowUI battleWindowUI;

        // Start is called before the first frame update
        void Start()
        {
            // UI初期化
            HideStatusWindow();
            HideCommandButtons();
        }

        /// <summary>
        /// ステータスウィンドウを隠す
        /// </summary>
        public void HideStatusWindow()
        {
            // オブジェクトを非アクティブ化
            statusWindow.SetActive(false);

            // テキストを非表示に
            nameText.enabled = false;
            hpText.enabled = false;
            atkText.enabled = false;
            defText.enabled = false;
        }

        /// <summary>
        /// コマンドボタンを非表示に
        /// </summary>
        public void HideCommandButtons()
        {
            commandButtons.SetActive(false);
        }

        /// <summary>
        /// ステータスウィンドウを表示する
        /// </summary>
        /// <param name="charaData">キャラクターデータ</param>
        public void ShowStatusWindow(Character.Character charaData)
        {
            // オブジェクトをアクティブに
            statusWindow.SetActive(true);

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
            defText.text = charaData.def.ToString();
        }

        /// <summary>
        /// コマンドボタンを表示
        /// </summary>
        public void ShowCommandButtons()
        {
            commandButtons.SetActive(true);
        }
    }
}