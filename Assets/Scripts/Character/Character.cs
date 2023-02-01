using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // ���C���J����

        public Animator animation;    // �A�j���[�V����

        // �L�����N�^�[�����ݒ�
        public int initPosition_X;    // ����X���W
        public int initPosition_Z;    // ����Z���W
        private const float initPosition_Y = 1.0f;  // Y���W�͌Œ�

        public bool isEnemy;               // �G���ǂ���
        public string characterName;       // �L�����N�^�[�̖��O
        public int maxHP;                  // �ő�HP
        public int atk;                    // �U����
        public int def;                    // �h���
        public Attribute attribute;        // ����
        public MoveType moveType;          // �ړ����@
        public Direction direction;        // �L�����N�^�[�̌����Ă������
        public SkillDefine.Skill skill;    // �X�L��

        public int activePoint;      // �s�����邽�߂̐��l�i3�ȏ�ōs���j
        public bool isActive;        // �s�����邩�ǂ���

        public GameObject statusUI;       // ���ݑI������Ă���L�����N�^�[��UI�\���p
        public RawImage image;
        public RenderTexture texture;     // �\������e�N�X�`��

        public GameObject selectingObj;   // �I������Ă��鎞�ɕ\�������I�u�W�F�N�g

        // �Q�[�����ɕω�����L�����N�^�[�f�[�^
        [HideInInspector]
        public int xPos;        // ���݂�x���W
        [HideInInspector]
        public int zPos;        // ���݂�z���W
        [HideInInspector]
        public int nowHP;       // ���݂�HP

        // �e���Ԉُ�
        public bool isSkillLock;    // �X�L���g�p�s�\
        public bool isDefBreak;     // �h��͂O�i�f�o�t�j

        // �A�j���[�V�������x
        const float animSpeed = 0.5f;

        public GameObject[] directionButtons;

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
            Bishop,     // �΂�
            Queen,      // �c�E���E�΂�
        }

        // �L�����N�^�[�̌�������
        public enum Direction
        {
            Forward,        // �O
            Backward,       // ���
            Right,          // �E
            Left,           // ��
        }

        // �L�����N�^�[�̌�����ݒ肷�邽�߂̕ϐ�
        private Vector3 rotation_F = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 rotation_B = new Vector3(0.0f, 180.0f, 0.0f);
        private Vector3 rotation_R = new Vector3(0.0f, 90.0f, 0.0f);
        private Vector3 rotation_L = new Vector3(0.0f, 270.0f, 0.0f);

        // Start is called before the first frame update
        void Start()
        {
            // �������W�ݒ�
            Vector3 position = new Vector3();
            position.x = initPosition_X;
            position.y = this.transform.position.y;    // 2D : 1.0f, 3D : 0.5f
            position.z = initPosition_Z;

            this.transform.position = position;

            // �X�P�[���̒���
            Vector3 scale = this.transform.localScale;
            this.transform.localScale = scale;

            // ���W�AHP�̏�����
            xPos = initPosition_X;
            zPos = initPosition_Z;
            nowHP = maxHP;

            // UI�̏�����
            statusUI.SetActive(false);
            selectingObj.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            /// �r���{�[�h���� ///
            Vector3 cameraPosition = mainCamera.transform.position;
            // �L�������n�ʂƐ����ɗ��悤��
            cameraPosition.y = this.transform.position.y;
            // �J�����̕�������
            //this.transform.LookAt(cameraPosition);

            // �I�����ꂽ�����������悤��
            switch(direction)
            {
                case Direction.Forward:
                    this.transform.eulerAngles = rotation_F;
                    break;
                case Direction.Backward:
                    this.transform.eulerAngles = rotation_B;
                    break;
                case Direction.Right:
                    this.transform.eulerAngles = rotation_R;
                    break;
                case Direction.Left:
                    this.transform.eulerAngles = rotation_L;
                    break;
            }
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
