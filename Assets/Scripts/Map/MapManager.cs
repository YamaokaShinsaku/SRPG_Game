using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapManager
{
    public class MapManager : MonoBehaviour
    {
        public MapBlock[,] mapData;     // マップデータ

        public Transform blockParent;       // ブロックの親オブジェクトのTransform
        public GameObject blockPrefab_Grass;    // 草ブロック
        public GameObject blockPrefab_Water;    // 水場ブロック

        // マップの横幅、立幅
        public const int MAP_WIDTH = 9;
        public const int MAP_HEIGHT = 9;

        // 草ブロックが生成される確率
        [SerializeField]
        private const int GENERATE_RATIO_GRASS = 90;

        // Start is called before the first frame update
        void Start()
        {
            // マップデータの初期化
            mapData = new MapBlock[MAP_WIDTH, MAP_HEIGHT];

            // 生成座標を設定
            Vector3 defaultPosition = new Vector3(0.0f, 0.0f, 0.0f);
            defaultPosition.x = -(MAP_WIDTH / 2);
            defaultPosition.z = -(MAP_HEIGHT / 2);

            // ブロック生成
            for (int i = 0; i < MAP_WIDTH; i++)
            {
                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    // ブロックの場所を決定
                    Vector3 position = defaultPosition;
                    position.x += i;
                    position.z += j;

                    // ブロックの種類を決定
                    int rand = Random.Range(0, 100);

                    // 草ブロック生成
                    bool isGrass = false;   // 草ブロック生成フラグ
                                            // 乱数値が草ブロック生成確率より低い時
                    if (rand < GENERATE_RATIO_GRASS)
                    {
                        isGrass = true;
                    }

                    // オブジェクトのクローンを生成
                    GameObject obj;
                    if (isGrass)
                    {
                        // blockParentの子にblockPrefab_Grassを生成
                        obj = Instantiate(blockPrefab_Grass, blockParent);
                    }
                    else
                    {
                        // blockParentの子にblockPrefab_Waterを生成
                        obj = Instantiate(blockPrefab_Water, blockParent);
                    }

                    // 座標更新
                    obj.transform.position = position;

                    // マップデータにブロックデータを格納
                    var mapBlock = obj.GetComponent<MapBlock>();
                    mapData[i, j] = mapBlock;

                    // ブロックデータ設定
                    mapBlock.xPos = (int)position.x;
                    mapBlock.zPos = (int)position.z;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 全てのオブジェクトの選択状態を解除する
        /// </summary>
        public void AllSelectionModeClear()
        {
            for (int i = 0; i < MAP_WIDTH; i++)
            {
                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    mapData[i, j].SetSelectionMode(false);
                }
            }
        }
    }

}