using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    /// <summary>
    /// スキル定義クラス
    /// </summary>
    public static class SkillDefine
    {
        // スキルの種類を定義
        public enum Skill
        {
            None,        // スキルなし
            Critical,    // 会心の一撃（クリティカル）
            DefBreak,    // シールド破壊
            Heal,        // ヒール
            FireBall,    // ファイアボール
        }

        /// <summary>
        /// スキルの定義と各データを紐づける
        /// </summary>
        /// スキル名
        public static Dictionary<Skill, string> dicSkillName = new Dictionary<Skill, string>()
        {
            { Skill.None,"スキルなし" },
            { Skill.Critical,"会心の一撃" },
            { Skill.DefBreak,"シールド破壊" },
            { Skill.Heal,"ヒール" },
            { Skill.FireBall,"ファイアボール" },
        };
        // スキルの説明文
        public static Dictionary<Skill, string> dicSkillInfo = new Dictionary<Skill, string>()
        {
            {Skill.None, "----" },
            {Skill.Critical, "ダメージ2倍の攻撃（1回限り）" },
            {Skill.DefBreak, "敵の攻撃力を0にする（ダメージは0）" },
            {Skill.Heal, "味方のHPを回復する" },
            {Skill.FireBall, "どの位置にいる敵にも攻撃できる" },
        };
    }
}
