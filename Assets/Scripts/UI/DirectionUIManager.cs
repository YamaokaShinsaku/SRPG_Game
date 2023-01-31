using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class DirectionUIManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;        // UIをおいているCanvas
        [SerializeField]
        private Transform targetTransform;    // 追従するオブジェクト
        [SerializeField]
        private APBattleManager apbattleManager;    // 現在選択されているキャラクターを取得

        // RectTransformを取得するための変数
        private RectTransform canvasRectTransform;
        private RectTransform myRectTransform;

        // UI画像のオフセット設定用
        public Vector3 offset = new Vector3(0, 0, 0);

        // Start is called before the first frame update
        void Start()
        {
            // コンポーネントを取得
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            myRectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            // UI画像の座標設定用変数
            Vector2 pos;
            if(apbattleManager.selectingCharacter)
            {
                targetTransform = apbattleManager.selectingCharacter.transform;
            }
            else
            {
                return;
            }

            // CanvasのRenderModeに応じてUIの座標の調整を行う
            switch (canvas.renderMode)
            {

                case RenderMode.ScreenSpaceOverlay:
                    myRectTransform.position =
                        RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position + offset);

                    break;

                case RenderMode.ScreenSpaceCamera:
                    Vector2 screenPos =
                        RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position + offset);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRectTransform, screenPos, Camera.main, out pos);
                    myRectTransform.localPosition = pos;
                    break;

                case RenderMode.WorldSpace:
                    myRectTransform.LookAt(Camera.main.transform);

                    break;
            }
        }
    }

}