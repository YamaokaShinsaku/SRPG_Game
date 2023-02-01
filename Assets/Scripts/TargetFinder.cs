using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TargetFinder
{
    /// <summary>
    /// �s���v��������N���X
    /// </summary>
    public class actionPlans
    {
        public Character.Character charaData;           // �s������G�L�����N�^�[
        public MapBlock toMoveBlock;                    // �ړ���̃u���b�N
        public Character.Character toAttaackChara;      // �U������L�����N�^�[
    }

    /// <summary>
    /// �U���\�ȍs����S�Č������A1�������_���ŕԂ�
    /// </summary>
    /// <param name="mapManager">�V�[������MapManager�Q��</param>
    /// <param name="characterManager">�V�[������CharacterManager�Q��</param>
    /// <param name="enemyCharas">�G�L�����N�^�[�̃��X�g</param>
    /// <returns></returns>
    public static actionPlans GetRandomactionPlans(MapManager.MapManager mapManager, 
        Character.CharacterManager characterManager, List<Character.Character> enemyCharas)
    {
        // �S�s���v�����i�U���\�ȑ��肪�����邲�Ƃɒǉ������j
        var actionPlans = new List<actionPlans>();
        // �ړ��͈̓��X�g
        var reachableBlocks = new List<MapBlock>();
        // �U���͈̓��X�g
        var attackableBlocks = new List<MapBlock>();

        // �S�s���v��������
        foreach(Character.Character enemyData in enemyCharas)
        {
            // �ړ��\�ȏꏊ���X�g���擾
            reachableBlocks =
                mapManager.SearchReachableBlocks(enemyData.xPos, enemyData.zPos);
            // ���ꂼ��̈ړ��\�ȏꏊ���Ƃ̏���
            foreach(MapBlock block in reachableBlocks)
            {
                // �U���\�ȏꏊ���X�g���擾
                attackableBlocks = mapManager.SearchAttackableBlocks(block.xPos, block.zPos);
                // ���ꂼ��̍U���\�ȏꏊ���Ƃ̏���
                foreach(MapBlock attackBlock in attackableBlocks)
                {
                    // �U���ł��鑊��L�����N�^�[��T��
                    Character.Character targetChara =
                        characterManager.GetCharacterData(attackBlock.xPos, attackBlock.zPos);
                    // ����L���������Ȃ��@���@�G�L�����ł͂Ȃ��Ƃ�
                    if(targetChara != null && !targetChara.isEnemy)
                    {
                        // �s���v������V�K�쐬����
                        var newPlan = new actionPlans();
                        newPlan.charaData = enemyData;
                        newPlan.toMoveBlock = block;
                        newPlan.toAttaackChara = targetChara;

                        // �S�s���v�������X�g�ɒǉ�
                        actionPlans.Add(newPlan);
                    }
                }
            }
        }

        // �����I����A�s���v������1�ł�����Ȃ�A���̂�����1�������_���ŕԂ�
        if(actionPlans.Count > 0)
        {
            return actionPlans[Random.Range(0, actionPlans.Count)];
        }
        // �s���v�������Ȃ��Ƃ�
        else
        {
            return null;
        }
    }
}
