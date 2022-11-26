using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // メインカメラ

        // キャラクター初期設定
        public int initPosition_X;
        public int initPosition_Z;
        private const float initPosition_Y = 1.0f;  // Y座標は固定

        public bool isEnemy;            // 敵かどうか
        public string characterName;    // キャラクターの名前
        public int maxHP;               // 最大HP
        public int atk;                 // 攻撃力
        public int def;                 // 防御力
        public Attribute attribute;     // 属性

        // ゲーム中に変化するキャラクターデータ
        [HideInInspector]
        public int xPos;        // 現在のx座標
        [HideInInspector]
        public int zPos;        // 現在のz座標
        [HideInInspector]
        public int nowHP;       // 現在のHP

        // 属性を定義
        public enum Attribute
        {
            Water,  // 水属性
            Fire,   // 火属性
            Wind,   // 風属性
            Soil,   // 土属性
        }

        // Start is called before the first frame update
        void Start()
        {
            // 初期座標設定
            Vector3 position = new Vector3();
            position.x = initPosition_X;
            position.y = initPosition_Y;
            position.z = initPosition_Z;

            this.transform.position = position;

            // オブジェクトを反転（ビルボード処理で一度反転するため）
            Vector2 scale = this.transform.localScale;
            scale.x *= -1.0f;
            this.transform.localScale = scale;

            xPos = initPosition_X;
            zPos = initPosition_Z;
            nowHP = maxHP;
        }

        // Update is called once per frame
        void Update()
        {
            /// ビルボード処理 ///
            Vector3 cameraPosition = mainCamera.transform.position;
            // キャラが地面と垂直に立つように
            cameraPosition.y = this.transform.position.y;
            // カメラの方を向く
            this.transform.LookAt(cameraPosition);
        }

        /// <summary>
        /// 指定座標にプレイヤーを移動させる
        /// </summary>
        /// <param name="targetPositionX">x座標</param>
        /// <param name="targetPositionZ">z座標</param>
        public void MovePosition(int targetPositionX, int targetPositionZ)
        {
            // 移動先座標への相対座標を取得
            Vector3 movePosition = Vector3.zero;
            movePosition.x = targetPositionX - xPos;
            movePosition.z = targetPositionZ - zPos;

            // 移動処理
            this.transform.position += movePosition;

            // キャラクターデータに位置を保存
            xPos = targetPositionX;
            zPos = targetPositionZ;
        }
    }
}