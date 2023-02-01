using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        this.gameObject.SetActive(false);
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
        float amount = (float)charaData.nowHP / charaData.maxHP;       // 表示中の FillAmount
        float endAmount = (float)nowHP / charaData.maxHP;    // アニメーション後の FillAmount

        // HPゲージを徐々に減少させるアニメーション
        DOTween.To(
            () => amount, (n) => amount = n,        // 変化させる変数を指定
            endAmount,          // 変化先の数値
            1.0f)               // アニメーション時間(秒)
            .OnUpdate(() =>     // アニメーション中毎フレーム実行される処理
            {
               // 最大値に対する現在のHPの割合を画像の FillAmount にセットする
               hpGageImg.fillAmount = amount;
           });

        // テキスト表示
        hpText.text = nowHP + "/" + charaData.maxHP;
        if(damegeValue >= 0)
        {
            damageText.text = damegeValue + "ダメージ！";
        }
        // HP回復時
        else
        {
            damageText.text = -damegeValue + "回復！";
        }
    }

    /// <summary>
    /// バトル結果ウィンドウを隠す
    /// </summary>
    public void HideWindow()
    {
        this.gameObject.SetActive(false);
    }
}
