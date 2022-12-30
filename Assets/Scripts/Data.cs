using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataManager
{
    /// <summary>
    /// シーン内に1つのみ存在する
    /// </summary>
    public class Data : MonoBehaviour
    {
        // シングルトン管理用変数
        // すべてのインスタンスでこの値を共有する
        [HideInInspector]
        public static bool instance = false;

        // プレイヤー強化データ
        public int addHP;    // 最大HP上昇量
        public int addAtk;   // 攻撃力上昇量
        public int addDef;   // 防御力上昇量

        // データのキー設定
        public const string Key_AddHP = "Key_AddHP";
        public const string Key_AddAtk = "Key_AddAtk";
        public const string Key_AddDef = "Key_AddDef";

        private void Awake()
        {
            // シングルトン用処理
            // すでにDataManagerがシーン内に存在している場合は自分を消去
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }

            // DataManagerが生成されたことをstatic変数に記録
            instance = true;
            // シーンをまたいでもこのオブジェクトが消去されないようにする
            DontDestroyOnLoad(this.gameObject);

            // セーブデータをPlayerPrefsから読み込み
            // キーに対応するデータを読み込む
            // データがない場合は、設定した初期値(0)を入れる
            addHP = PlayerPrefs.GetInt(Key_AddHP, 0);
            addAtk = PlayerPrefs.GetInt(Key_AddAtk, 0);
            addDef = PlayerPrefs.GetInt(Key_AddDef, 0);
        }

        /// <summary>
        /// 現在のプレイヤー強化データをPlayerPrefsに保存する
        /// </summary>
        public void WriteSaveData()
        {
            // データをキーとともに変更
            PlayerPrefs.SetInt(Key_AddHP, addHP);
            PlayerPrefs.SetInt(Key_AddAtk, addAtk);
            PlayerPrefs.SetInt(Key_AddDef, addDef);
            // 変更を保存
            PlayerPrefs.Save();
        }
    }

}