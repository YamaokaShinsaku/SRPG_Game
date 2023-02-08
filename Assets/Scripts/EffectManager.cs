using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectManager
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject backAttackEffect;    // �o�b�N�A�^�b�N�G�t�F�N�g
        [SerializeField]
        private GameObject dethEffect;          // ���S�G�t�F�N�g
        [SerializeField]
        private GameObject damageEffect;        // �_���[�W�G�t�F�N�g

        public GameObject Target;
        public TextMesh damageText;
        private Vector3 offset;

        // Start is called before the first frame update
        void Start()
        {
            backAttackEffect.SetActive(false);
            dethEffect.SetActive(false);
            damageEffect.SetActive(false);
        }

        /// <summary>
        /// �o�b�N�A�^�b�N�G�t�F�N�g�̍Đ�
        /// </summary>
        /// <param name="selectintCharacter">�I�𒆂̃L�����N�^�[</param>
        public void PlayBackAttackEffect(Character.Character selectingCharacter)
        {
            if(selectingCharacter.zPos >= 0.0f)
            {
                // �I�t�Z�b�g��ݒ�
                offset = new Vector3(-0.5f, 3.0f, -3.0f);
            }
            else if(selectingCharacter.zPos < 0.0f)
            {
                // �I�t�Z�b�g��ݒ�
                offset = new Vector3(-0.5f, 3.0f, 0.0f);
            }




            // �p�x�𒲐��iTarget�̂ق��������悤�Ɂj
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // �G�t�F�N�g�̕\�����W��ݒ�
            backAttackEffect.transform.rotation = Rotation;
            backAttackEffect.transform.position = selectingCharacter.transform.position + offset;

            // �G�t�F�N�g���Đ�
            backAttackEffect.SetActive(true);

            //Debug.Log("Effects");
        }

        /// <summary>
        /// ���S���̃G�t�F�N�g�Đ�
        /// </summary>
        /// <param name="selectingCharacter">�I�𒆂̃L�����N�^�[</param>
        public void PlayDethEffect(Character.Character selectingCharacter)
        {
            GameObject clone = Instantiate(dethEffect);
            // �I�t�Z�b�g��ݒ�
            offset = new Vector3(0.0f, 0.5f, 0.0f);
            // �G�t�F�N�g�̕\�����W��ݒ�
            clone.transform.position = selectingCharacter.transform.position + offset;

            // �G�t�F�N�g���Đ�
            clone.SetActive(true);
        }

        public void PlayDamageEffect(Character.Character damageCharacter, float damage)
        {
            if (damageCharacter.zPos >= 0.0f)
            {
                // �I�t�Z�b�g��ݒ�
                offset = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (damageCharacter.zPos < 0.0f)
            {
                // �I�t�Z�b�g��ݒ�
                offset = new Vector3(0.0f, 1.0f, 0.0f);
            }

            // �p�x�𒲐��iTarget�̂ق��������悤�Ɂj
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // �G�t�F�N�g�̕\�����W��ݒ�
            damageEffect.transform.rotation = Rotation;
            damageEffect.transform.position = damageCharacter.transform.position + offset;

            damageText.text = damage.ToString();

            // �G�t�F�N�g���Đ�
            damageEffect.SetActive(true);
        }
    }
}
