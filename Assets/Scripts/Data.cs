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
        public static bool instance = false;

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
        }
    }

}