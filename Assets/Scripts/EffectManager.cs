using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectManager
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject backAttackEffects;    // �o�b�N�A�^�b�N�G�t�F�N�g

        public GameObject Target;
        private Vector3 offset;

        // Start is called before the first frame update
        void Start()
        {
            backAttackEffects.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// �o�b�N�A�^�b�N�G�t�F�N�g�̍Đ�
        /// </summary>
        /// <param name="selectintCharacter">�I�𒆂̃L�����N�^�[</param>
        public void PlayBackAttackEffect(Character.Character selectintCharacter)
        {
            // �I�t�Z�b�g��ݒ�
            offset = new Vector3(-0.5f, 3.0f, 0.0f);

            // �p�x�𒲐��iTarget�̂ق��������悤�Ɂj
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // �G�t�F�N�g�̕\�����W��ݒ�
            backAttackEffects.transform.rotation = Rotation;
            backAttackEffects.transform.position = selectintCharacter.transform.position + offset;

            // �G�t�F�N�g���Đ�
            backAttackEffects.SetActive(true);

            //Debug.Log("Effects");
        }
    }
}

