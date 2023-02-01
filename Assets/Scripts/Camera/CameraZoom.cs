using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraMove
{
    /// <summary>
    /// �X�}�z�ł̎������ɍs���Y�[������
    /// </summary>
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;      // ���C���J����

        const float zoomSpeed = 0.1f;       // �Y�[�����x
        const float minFOV = 40.0f;          // �J�����̍ŏ�����
        const float maxFOV = 60.0f;         // �J�����̍ő压��

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // �}���`�^�b�`�łȂ��Ȃ�I��(�Q�_�����^�b�`)
            if(Input.touchCount != 2)
            {
                return;
            }

            // �Q�_�̃^�b�`�����擾
            var touchData0 = Input.GetTouch(0);
            var touchData1 = Input.GetTouch(1);

            // �P�t���[���O�̂Q�_�ԋ��������߂�
            // deltaPosition�ɂ�1�t���[���O�̃^�b�`�ʒu�������Ă���
            float oldTouchDistance = Vector2.Distance(
                touchData0.position - touchData0.deltaPosition,
                touchData1.position - touchData1.deltaPosition);

            // ���݂�2�_�ԋ��������߂�
            float currentTouchDistance = Vector2.Distance(touchData0.position, touchData1.position);

            // 2�_�ԋ����̕ω��ʂɉ����ăY�[������
            float distanceMoved = oldTouchDistance - currentTouchDistance;
            mainCamera.fieldOfView += distanceMoved * zoomSpeed;

            // �J�����̎�����w��͈̔͂Ɏ��߂�
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
        }
    }
}
