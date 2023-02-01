using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // メインカメラ

        public Animator animation;    // アニメーション

        // キャラクター初期設定
        public int initPosition_X;    // 初期X座標
        public int initPosition_Z;    // 初期Z座標
        private const float initPosition_Y = 1.0f;  // Y座標は固定

        public bool isEnemy;               // 敵かどうか
        public string characterName;       // キャラクターの名前
        public int maxHP;                  // 最大HP
        public int atk;                    // 攻撃力
        public int def;                    // 防御力
        public Attribute attribute;        // 属性
        public MoveType moveType;          // 移動方法
        public Direction direction;        // キャラクターの向いている方向
        public SkillDefine.Skill skill;    // スキル

        public int activePoint;      // 行動するための数値（3以上で行動）
        public bool isActive;        // 行動するかどうか

        public GameObject statusUI;       // 現在選択されているキャラクターのUI表示用
        public RawImage image;
        public RenderTexture texture;     // 表示するテクスチャ

        public GameObject selectingObj;   // 選択されている時に表示されるオブジェクト

        // ゲーム中に変化するキャラクターデータ
        [HideInInspector]
        public int xPos;        // 現在のx座標
        [HideInInspector]
        public int zPos;        // 現在のz座標
        [HideInInspector]
        public int nowHP;       // 現在のHP

        // 各種状態異常
        public bool isSkillLock;    // スキル使用不可能
        public bool isDefBreak;     // 防御力０（デバフ）

        // アニメーション速度
        const float animSpeed = 0.5f;

        public GameObject[] directionButtons;

        // 属性を定義
        public enum Attribute
        {
            Water,  // 水属性
            Fire,   // 火属性
            Wind,   // 風属性
            Soil,   // 土属性
        }

        // キャラクターの移動法を定義
        public enum MoveType
        {
            Rook,       // 縦・横
            Bishop,     // 斜め
            Queen,      // 縦・横・斜め
        }

        // キャラクターの向く方向
        public enum Direction
        {
            Forward,        // 前
            Backward,       // 後ろ
            Right,          // 右
            Left,           // 左
        }

        // キャラクターの向きを設定するための変数
        private Vector3 rotation_F = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 rotation_B = new Vector3(0.0f, 180.0f, 0.0f);
        private Vector3 rotation_R = new Vector3(0.0f, 90.0f, 0.0f);
        private Vector3 rotation_L = new Vector3(0.0f, 270.0f, 0.0f);

        // Start is called before the first frame update
        void Start()
        {
            // 初期座標設定
            Vector3 position = new Vector3();
            position.x = initPosition_X;
            position.y = this.transform.position.y;    // 2D : 1.0f, 3D : 0.5f
            position.z = initPosition_Z;

            this.transform.position = position;

            // スケールの調整
            Vector3 scale = this.transform.localScale;
            this.transform.localScale = scale;

            // 座標、HPの初期化
            xPos = initPosition_X;
            zPos = initPosition_Z;
            nowHP = maxHP;

            // UIの初期化
            statusUI.SetActive(false);
            selectingObj.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            /// ビルボード処理 ///
            Vector3 cameraPosition = mainCamera.transform.position;
            // キャラが地面と垂直に立つように
            cameraPosition.y = this.transform.position.y;
            // カメラの方を向く
            //this.transform.LookAt(cameraPosition);

            // 選択された向きを向くように
            switch(direction)
            {
                case Direction.Forward:
                    this.transform.eulerAngles = rotation_F;
                    break;
                case Direction.Backward:
                    this.transform.eulerAngles = rotation_B;
                    break;
                case Direction.Right:
                    this.transform.eulerAngles = rotation_R;
                    break;
                case Direction.Left:
                    this.transform.eulerAngles = rotation_L;
                    break;
            }
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

            // DoTweenを利用した移動アニメーション
            this.transform.DOMove(movePosition, animSpeed)
                .SetEase(Ease.Linear)   // 変化の度合いを設定
                .SetRelative();         // パラメーターを相対指定にする


            // 移動処理
            //this.transform.position += movePosition;

            // キャラクターデータに位置を保存
            xPos = targetPositionX;
            zPos = targetPositionZ;
        }

        /// <summary>
        /// 近接攻撃アニメーション
        /// </summary>
        /// <param name="targetChara">攻撃するキャラクター</param>
        public void AttackAnimation(Character targetChara)
        {
            // 攻撃アニメーション
            // 攻撃するキャラクターの位置へジャンプで近づき、同じ動作で戻る
            this.transform.DOJump(targetChara.transform.position,
                1.0f,       // ジャンプの高さ
                1,          // ジャンプ回数
                animSpeed)
                .SetEase(Ease.Linear)           // 変化の度合いを設定
                .SetLoops(2, LoopType.Yoyo);    // ループ回数・方式を設定
        }
    }
}
