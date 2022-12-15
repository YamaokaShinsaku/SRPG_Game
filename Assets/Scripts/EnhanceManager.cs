using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DataManager
{
    /// <summary>
    /// Dataクラスの上昇量の値の変更をする
    /// </summary>
    public class EnhanceManager : MonoBehaviour
    {
        // Dataクラス
        private Data data;

        // UIボタン
        public List<Button> EnhanceButtons;    // ステータス上昇ボタン
        public Button GoGameButton;            // もう一度プレイボタン

        // Start is called before the first frame update
        void Start()
        {
            // DataManagerからデータ管理クラスを取得
            data = GameObject.Find("DataManager").GetComponent<Data>();

            // いずれかを強化するまでは「もう一度プレイボタン」を押せないようにする
            GoGameButton.interactable = false;
        }

        /// <summary>
        /// 最大HPを上昇させる
        /// </summary>
        public void AddHP()
        {
            // 強化処理
            data.addHP += 2;

            // 強化終了時の処理
            EnhanceComplete();
        }

        /// <summary>
        /// 攻撃力を上昇させる
        /// </summary>
        public void AddAtk()
        {
            // 強化処理
            data.addAtk += 1;

            // 強化終了時の処理
            EnhanceComplete();
        }

        /// <summary>
        /// 防御力を上昇させる
        /// </summary>
        public void AddDef()
        {
            // 強化処理
            data.addDef += 1;

            // 強化終了時の処理
            EnhanceComplete();
        }

        /// <summary>
        /// 強化完了時の共通処理
        /// </summary>
        private void EnhanceComplete()
        {
            // 強化ボタンを押せないようにする
            foreach(Button button in EnhanceButtons)
            {
                button.interactable = false;
            }

            // もう一度プレイボタンを押せるようにする
            GoGameButton.interactable = true;

            // 変更をデータに保存
            data.WriteSaveData();
        }

        /// <summary>
        /// ゲームシーンに切り替える
        /// </summary>
        public void GoGameScene()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
