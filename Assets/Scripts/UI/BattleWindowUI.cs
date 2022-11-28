using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindowUI : MonoBehaviour
{
    /// バトル結果表示ウィンドウ ///
    public Text nameText;
    public Image hpGageImg;     // HPゲージ画像
    public Text hpText;         // HPテキスト
    public Text damageText;     // ダメージテキスト


    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// バトル結果ウィンドウを表示する
    /// </summary>
    /// <param name="charaData">攻撃されたキャラクターのデータ</param>
    /// <param name="damegeValue">ダメージ量</param>
    public void ShowWindow(Character.Character charaData, int damegeValue)
    {
        // オブジェクトのアクティブ化
        this.gameObject.SetActive(true);

        // 名前テキストの表示
        nameText.text = charaData.characterName;

        // ダメージを計算して、残りHPを取得
        int nowHP = charaData.nowHP - damegeValue;
        // HPが0～最大値の範囲に収まるように補正
        nowHP = Mathf.Clamp(nowHP, 0, charaData.maxHP);

        // HPゲージ表示
        float ratio = (float)nowHP / charaData.maxHP;
        // 現在のHPの割合をゲージ画像の fillAmount　にセットする
        hpGageImg.fillAmount = ratio;

        // テキスト表示
        hpText.text = nowHP + "/" + charaData.maxHP;
        damageText.text = damegeValue + "ダメージ！";
    }

    /// <summary>
    /// バトル結果ウィンドウを隠す
    /// </summary>
    public void HideWindow()
    {
        this.gameObject.SetActive(false);
    }
}
