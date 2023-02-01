using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CameraMove
{
    public class CameraController : MonoBehaviour
    {
        private bool isCameraRotate;    // カメラ回転フラグ
        private bool isInversion;           // カメラ回転方向反転フラグ

        const float rotationSpeed = 30.0f;      // 回転速度

        // Update is called once per frame
        void Update()
        {
            // カメラ回転
            if (isCameraRotate)
            {
                // 回転速度を計算
                float speed = rotationSpeed * Time.deltaTime;
                // 回転方向反転フラグ(isInversion)が立っているなら、速度を反転
                if (isInversion)
                {
                    speed *= -1.0f;
                }

                // 起点の位置を中心にカメラを回転移動させる
                this.transform.RotateAround(Vector3.zero, Vector3.up, speed);
            }
        }

        /// <summary>
        /// カメラ移動開始
        /// </summary>
        /// <param name="rightRotate">右向きの移動フラグ（右移動中：true）</param>
        public void CameraRotateStart(bool rightRotate)
        {
            // カメラ回転フラグをtrueに
            isCameraRotate = true;
            // 回転方向反対フラグを適用する
            isInversion = rightRotate;
        }

        /// <summary>
        /// カメラ移動終了
        /// </summary>
        public void CameraRotateEnd()
        {
            // カメラ回転フラグをfalseに
            isCameraRotate = false;
        }
    }
}
