using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager.MapManager mapManager;

        // Start is called before the first frame update
        void Start()
        {
            mapManager = GetComponent<MapManager.MapManager>();
        }

        // Update is called once per frame
        void Update()
        {
            // タップ先を検出
            if(Input.GetMouseButtonDown(0))
            {
                GetMapObjects();
            }
        }

        /// <summary>
        /// タップした場所のオブジェクトを取得
        /// </summary>
        private void GetMapObjects()
        {
            GameObject targetObject = null;     // タップしたオブジェクト

            // タップした方向にカメラからRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            // Rayの当たる位置に存在するオブジェクトを取得
            if(Physics.Raycast(ray, out hit))
            {
                targetObject = hit.collider.gameObject;
            }

            // 対象オブジェクトが存在する場合の処理
            if(targetObject != null)
            {
                SelectBlock(targetObject.GetComponent<MapBlock>());
            }
        }

        /// <summary>
        /// 指定したブロックを選択状態にする
        /// </summary>
        /// <param name="targetBlock">対象のオブジェクトデータ</param>
        private void SelectBlock(MapBlock targetObject)
        {
            // 全ブロックの選択状態を解除する
            mapManager.AllSelectionModeClear();

            // ブロックを選択状態にする
            targetObject.SetSelectionMode(true);

            Debug.Log("オブジェクトがタップされました \nブロック座標 : "
                + targetObject.transform.position);
        }
    }

}