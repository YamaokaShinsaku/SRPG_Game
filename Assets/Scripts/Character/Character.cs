using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // メインカメラ

        // キャラクター初期位置
        public int initPosition_X;
        public int initPosition_Z;
        private const float initPosition_Y = 1.0f;  // Y座標は固定

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
    }

}