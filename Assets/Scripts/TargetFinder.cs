using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TargetFinder
{
    /// <summary>
    /// 行動プラン決定クラス
    /// </summary>
    public class actionPlans
    {
        public Character.Character charaData;           // 行動する敵キャラクター
        public MapBlock toMoveBlock;                    // 移動先のブロック
        public Character.Character toAttaackChara;      // 攻撃するキャラクター
    }

    /// <summary>
    /// 攻撃可能な行動を全て検索し、1つをランダムで返す
    /// </summary>
    /// <param name="mapManager">シーン内のMapManager参照</param>
    /// <param name="characterManager">シーン内のCharacterManager参照</param>
    /// <param name="enemyCharas">敵キャラクターのリスト</param>
    /// <returns></returns>
    public static actionPlans GetRandomactionPlans(MapManager.MapManager mapManager, 
        Character.CharacterManager characterManager, List<Character.Character> enemyCharas)
    {
        // 全行動プラン（攻撃可能な相手が見つかるごとに追加される）
        var actionPlans = new List<actionPlans>();
        // 移動範囲リスト
        var reachableBlocks = new List<MapBlock>();
        // 攻撃範囲リスト
        var attackableBlocks = new List<MapBlock>();

        // 全行動プラン検索
        foreach(Character.Character enemyData in enemyCharas)
        {
            // 移動可能な場所リストを取得
            reachableBlocks =
                mapManager.SearchReachableBlocks(enemyData.xPos, enemyData.zPos);
            // それぞれの移動可能な場所ごとの処理
            foreach(MapBlock block in reachableBlocks)
            {
                // 攻撃可能な場所リストを取得
                attackableBlocks = mapManager.SearchAttackableBlocks(block.xPos, block.zPos);
                // それぞれの攻撃可能な場所ごとの処理
                foreach(MapBlock attackBlock in attackableBlocks)
                {
                    // 攻撃できる相手キャラクターを探す
                    Character.Character targetChara =
                        characterManager.GetCharacterData(attackBlock.xPos, attackBlock.zPos);
                    // 相手キャラがいない　かつ　敵キャラではないとき
                    if(targetChara != null && !targetChara.isEnemy)
                    {
                        // 行動プランを新規作成する
                        var newPlan = new actionPlans();
                        newPlan.charaData = enemyData;
                        newPlan.toMoveBlock = block;
                        newPlan.toAttaackChara = targetChara;

                        // 全行動プランリストに追加
                        actionPlans.Add(newPlan);
                    }
                }
            }
        }

        // 検索終了後、行動プランが1つでもあるなら、そのうちの1つをランダムで返す
        if(actionPlans.Count > 0)
        {
            return actionPlans[Random.Range(0, actionPlans.Count)];
        }
        // 行動プランがないとき
        else
        {
            return null;
        }
    }
}
