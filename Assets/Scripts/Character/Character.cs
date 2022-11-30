using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // ���C���J����

        // �L�����N�^�[�����ݒ�
        public int initPosition_X;
        public int initPosition_Z;
        private const float initPosition_Y = 1.0f;  // Y���W�͌Œ�

        public bool isEnemy;                // �G���ǂ���
        public string characterName;    // �L�����N�^�[�̖��O
        public int maxHP;                    // �ő�HP
        public int atk;                          // �U����
        public int def;                          // �h���
        public Attribute attribute;         // ����
        public MoveType moveType;     // �ړ����@

        // �Q�[�����ɕω�����L�����N�^�[�f�[�^
        [HideInInspector]
        public int xPos;        // ���݂�x���W
        [HideInInspector]
        public int zPos;        // ���݂�z���W
        [HideInInspector]
        public int nowHP;       // ���݂�HP

        // �A�j���[�V�������x
        const float animSpeed = 0.5f;

        // �������`
        public enum Attribute
        {
            Water,  // ������
            Fire,   // �Α���
            Wind,   // ������
            Soil,   // �y����
        }

        // �L�����N�^�[�̈ړ��@���`
        public enum MoveType
        {
            Rook,       // �c�E��
            Bishop,    // �΂�
            Queen,    // �c�E���E�΂�
        }

        // Start is called before the first frame update
        void Start()
        {
            // �������W�ݒ�
            Vector3 position = new Vector3();
            position.x = initPosition_X;
            position.y = initPosition_Y;
            position.z = initPosition_Z;

            this.transform.position = position;

            // �I�u�W�F�N�g�𔽓]�i�r���{�[�h�����ň�x���]���邽�߁j
            Vector2 scale = this.transform.localScale;
            scale.x *= -1.0f;
            this.transform.localScale = scale;

            xPos = initPosition_X;
            zPos = initPosition_Z;
            nowHP = maxHP;
        }

        // Update is called once per frame
        void Update()
        {
            /// �r���{�[�h���� ///
            Vector3 cameraPosition = mainCamera.transform.position;
            // �L�������n�ʂƐ����ɗ��悤��
            cameraPosition.y = this.transform.position.y;
            // �J�����̕�������
            this.transform.LookAt(cameraPosition);
        }

        /// <summary>
        /// �w����W�Ƀv���C���[���ړ�������
        /// </summary>
        /// <param name="targetPositionX">x���W</param>
        /// <param name="targetPositionZ">z���W</param>
        public void MovePosition(int targetPositionX, int targetPositionZ)
        {
            // �ړ�����W�ւ̑��΍��W���擾
            Vector3 movePosition = Vector3.zero;
            movePosition.x = targetPositionX - xPos;
            movePosition.z = targetPositionZ - zPos;

            // DoTween�𗘗p�����ړ��A�j���[�V����
            this.transform.DOMove(movePosition, animSpeed)
                .SetEase(Ease.Linear)   // �ω��̓x������ݒ�
                .SetRelative();         // �p�����[�^�[�𑊑Ύw��ɂ���

            // �ړ�����
            //this.transform.position += movePosition;

            // �L�����N�^�[�f�[�^�Ɉʒu��ۑ�
            xPos = targetPositionX;
            zPos = targetPositionZ;
        }

        /// <summary>
        /// �ߐڍU���A�j���[�V����
        /// </summary>
        /// <param name="targetChara">�U������L�����N�^�[</param>
        public void AttackAnimation(Character targetChara)
        {
            // �U���A�j���[�V����
            // �U������L�����N�^�[�̈ʒu�փW�����v�ŋ߂Â��A��������Ŗ߂�
            this.transform.DOJump(targetChara.transform.position,
                1.0f,       // �W�����v�̍���
                1,          // �W�����v��
                animSpeed)
                .SetEase(Ease.Linear)           // �ω��̓x������ݒ�
                .SetLoops(2, LoopType.Yoyo);    // ���[�v�񐔁E������ݒ�
        }
    }
}