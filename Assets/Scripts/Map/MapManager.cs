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
        private const int GENERATE_RATIO_GRASS = 100;

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

        /// <summary>
        /// 全てのオブジェクトの選択状態を解除する
        /// </summary>
        public void AllSelectionModeClear()
        {
            for (int i = 0; i < MAP_WIDTH; i++)
            {
                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    mapData[i, j].SetSelectionMode(MapBlock.Highlight.Off);
                }
            }
        }

        /// <summary>
        /// 渡された位置からキャラクターが到達できる場所のブロックをリストにして返す
        /// </summary>
        /// <param name="xPos">x座標</param>
        /// <param name="zPos">z座標</param>
        /// <returns>条件を満たすブロックのリスト</returns>
        public List<MapBlock> SearchReachableBlocks(int xPos, int zPos)
        {
            // 条件を満たすブロックのリスト
            var results = new List<MapBlock>();

            // 基点となるブロックの配列内番号を検索
            int baseX = -1, baseZ = -1;     // 配列内番号
            // 検索
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    // 指定された座標に一致するマップブロックが見つかった時
                    if((mapData[i,j].xPos == xPos) && (mapData[i, j].zPos == zPos))
                    {
                        // 配列内番号を取得
                        baseX = i;
                        baseZ = j;

                        // 2個目のループを抜ける
                        break;
                    }
                }
                // すでに発見済みなら一個目のループを抜ける
                if (baseX != -1)
                {
                    break;
                }
            }

            // 移動するキャラクターの移動方法を取得
            var moveType = Character.Character.MoveType.Rook;   // 移動方法
            var moveChara =                                     // 移動するキャラクター
                GetComponent<Character.CharacterManager>().GetCharacterData(xPos, zPos);
            // 移動するキャラクターが存在する時
            if(moveChara != null)
            {
                // キャラクターデータから移動方法を取得
                moveType = moveChara.moveType;
            }

            // キャラクターの移動方法に合わせて異なる方向のブロックデータを取得
            // Rook, Queen
            if(moveType == Character.Character.MoveType.Rook
                || moveType == Character.Character.MoveType.Queen)
            {
                // X+ 方向
                for (int i = baseX + 1; i < MAP_WIDTH; i++)
                {
                    if (AddReachableList(results, mapData[i, baseZ]))
                    {
                        break;
                    }
                }
                // X- 方向
                for (int i = baseX - 1; i >= 0; i--)
                {
                    if (AddReachableList(results, mapData[i, baseZ]))
                    {
                        break;
                    }
                }
                // Z+ 方向
                for (int j = baseZ + 1; j < MAP_HEIGHT; j++)
                {
                    if (AddReachableList(results, mapData[baseX, j]))
                    {
                        break;
                    }
                }
                // Z- 方向
                for (int j = baseZ - 1; j >= 0; j--)
                {
                    if (AddReachableList(results, mapData[baseX, j]))
                    {
                        break;
                    }
                }
            }

            // Bishop, Queen
            if(moveType == Character.Character.MoveType.Bishop 
                || moveType == Character.Character.MoveType.Queen)
            {
                // X+Z+ 方向
                for (int i = baseX + 1, j = baseZ + 1; i < MAP_WIDTH && j < MAP_HEIGHT; i++, j++)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X-Z+ 方向
                for (int i = baseX - 1, j = baseZ + 1; i >= 0 && j < MAP_HEIGHT; i--, j++)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X+Z- 方向
                for (int i = baseX + 1, j = baseZ - 1; i < MAP_WIDTH && j >= 0; i++, j--)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X-Z- 方向
                for (int i = baseX - 1, j = baseZ - 1; i >= 0 && j >= 0; i--, j--)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
            }

            // 足元のブロック
            results.Add(mapData[baseX, baseZ]);

            return results;
        }

        /// <summary>
        /// 指定したブロックを到達可能ブロックリストに追加する
        /// （キャラクター到達ブロック検索用）
        /// </summary>
        /// <param name="reachableList">到達可能ブロックリスト</param>
        /// <param name="targetBlock">指定のブロック</param>
        /// <returns>行き止まりかどうか 　true : 行き止まり</returns>
        private bool AddReachableList(List<MapBlock> reachableList, MapBlock targetBlock)
        {
            // 対象のブロックが通行不可ならそこを行き止まりとする
            if(!targetBlock.isPassable)
            {
                return true;
            }

            // 対象の位置にほかのキャラクターが存在しているなら、
            // 到達不可能にして終了
            var charaData =
                GetComponent<Character.CharacterManager>().GetCharacterData(targetBlock.xPos, targetBlock.zPos);
            if(charaData != null)
            {
                return false;
            }

            // 到達可能ブロックリストに追加する
            reachableList.Add(targetBlock);

            return false;
        }

        /// <summary>
        /// 指定位置から攻撃できる場所のマップブロックをリストにして返す
        /// </summary>
        /// <param name="xPos">基点x座標</param>
        /// <param name="zPos">基点z座標</param>
        /// <returns>攻撃できるブロックのリスト</returns>
        public List<MapBlock> SearchAttackableBlocks(int xPos, int zPos)
        {
            // 条件を満たすマップブロックのリスト
            var results = new List<MapBlock>();

            // 基点となるブロックの配列内番号を検索
            int baseX = -1, baseZ = -1;
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    if((mapData[i,j].xPos == xPos) && (mapData[i, j].zPos == zPos))
                    {
                        // 一致するマップブロックがあれば配列内番号を取得
                        baseX = i;
                        baseZ = j;

                        // 2個目のループを抜ける
                        break;
                    }
                }
                // すでに発見済みなら
                if (baseX != -1)
                {
                    // 1個目のループを抜ける
                    break;
                }
            }

            // 4方向に1マス進んだ一のブロックをセット
            // 縦横
            // X+方向
            AddAttackableList(results, baseX + 1, baseZ);
            // X-方向
            AddAttackableList(results, baseX - 1, baseZ);
            // Z+方向
            AddAttackableList(results, baseX, baseZ + 1);
            // Z-方向
            AddAttackableList(results, baseX, baseZ - 1);
            // 斜め
            // X+Z+方向
            AddAttackableList(results, baseX + 1, baseZ + 1);
            // X-Z+方向
            AddAttackableList(results, baseX - 1, baseZ + 1);
            // X+Z-方向
            AddAttackableList(results, baseX + 1, baseZ - 1);
            // X-Z-方向
            AddAttackableList(results, baseX - 1, baseZ - 1);

            return results;
        }

        /// <summary>
        /// マップデータの指定された配列内番号に対応するブロックを
        /// 攻撃可能ブロックリストに追加する
        /// </summary>
        /// <param name="attackableList">攻撃可能ブロックリスト</param>
        /// <param name="indexX">X方向の配列内番号</param>
        /// <param name="indexZ">Z方向の配列内番号</param>
        public void AddAttackableList(List<MapBlock> attackableList, int indexX, int indexZ)
        {
            // 指定番号が配列の外に出ていたら追加せずに終了
            if (indexX < 0 || indexX >= MAP_WIDTH || 
                indexZ < 0 || indexZ >= MAP_HEIGHT)
            {
                return;
            }

            // 攻撃可能ブロックリストに追加する
            attackableList.Add(mapData[indexX, indexZ]);
        }

        /// <summary>
        /// マップデータ配列をリストにして返す
        /// </summary>
        /// <returns>マップデータのリスト</returns>
        public List<MapBlock> MapBlocksToList()
        {
            // 結果用リスト
            var results = new List<MapBlock>();

            // マップデータ配列の中身を順番にリストに格納
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    results.Add(mapData[i, j]);
                }
            }

            return results;
        }

    }

}