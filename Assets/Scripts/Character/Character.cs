using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;  // ���C���J����

        // �L�����N�^�[�����ʒu
        public int initPosition_X;
        public int initPosition_Z;
        private const float initPosition_Y = 1.0f;  // Y���W�͌Œ�

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
    }

}