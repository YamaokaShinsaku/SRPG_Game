using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    /// <summary>
    /// �X�L����`�N���X
    /// </summary>
    public static class SkillDefine
    {
        // �X�L���̎�ނ��`
        public enum Skill
        {
            None,        // �X�L���Ȃ�(�ʏ�U��)
            Critical,    // ��S�̈ꌂ�i�N���e�B�J���j
            DefBreak,    // �V�[���h�j��
            Heal,        // �q�[��
            FireBall,    // �t�@�C�A�{�[��
        }

        /// <summary>
        /// �X�L���̒�`�Ɗe�f�[�^��R�Â���
        /// </summary>
        /// �X�L����
        public static Dictionary<Skill, string> dicSkillName = new Dictionary<Skill, string>()
        {
            { Skill.None,"�X�L���Ȃ�" },
            { Skill.Critical,"��S�̈ꌂ" },
            { Skill.DefBreak,"�V�[���h�j��" },
            { Skill.Heal,"�q�[��" },
            { Skill.FireBall,"�t�@�C�A�{�[��" },
        };
        // �X�L���̐�����
        public static Dictionary<Skill, string> dicSkillInfo = new Dictionary<Skill, string>()
        {
            {Skill.None, "----" },
            {Skill.Critical, "�_���[�W2�{�̍U���i1�����j" },
            {Skill.DefBreak, "�G�̖h��͂�0�ɂ���i�^����_���[�W��0�j" },
            {Skill.Heal, "������HP���񕜂���" },
            {Skill.FireBall, "�ǂ̈ʒu�ɂ���G�ɂ��U���ł���" },
        };
    }
}
