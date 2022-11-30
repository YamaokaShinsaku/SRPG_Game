using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraMove
{
    /// <summary>
    /// スマホでの実装時に行うズーム処理
    /// </summary>
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;      // メインカメラ

        const float zoomSpeed = 0.1f;       // ズーム速度
        const float minFOV = 40.0f;          // カメラの最小視野
        const float maxFOV = 60.0f;         // カメラの最大視野

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // マルチタッチでないなら終了(２点同時タッチ)
            if(Input.touchCount != 2)
            {
                return;
            }

            // ２点のタッチ情報を取得
            var touchData0 = Input.GetTouch(0);
            var touchData1 = Input.GetTouch(1);

            // １フレーム前の２点間距離を求める
            // deltaPositionには1フレーム前のタッチ位置が入っている
            float oldTouchDistance = Vector2.Distance(
                touchData0.position - touchData0.deltaPosition,
                touchData1.position - touchData1.deltaPosition);

            // 現在の2点間距離を求める
            float currentTouchDistance = Vector2.Distance(touchData0.position, touchData1.position);

            // 2点間距離の変化量に応じてズームする
            float distanceMoved = oldTouchDistance - currentTouchDistance;
            mainCamera.fieldOfView += distanceMoved * zoomSpeed;

            // カメラの視野を指定の範囲に収める
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
        }
    }
}
