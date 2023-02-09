using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CameraMove
{
    public class CameraController : MonoBehaviour
    {
        private bool isCameraRotate;    // �J������]�t���O
        private bool isInversion;           // �J������]�������]�t���O

        const float rotationSpeed = 30.0f;      // ��]���x

        [SerializeField]
        private Transform targetTransform;

        public Transform prevCameraTransform;

        private Vector3 velocity = Vector3.zero;

        // Update is called once per frame
        void Update()
        {
            // �J������]
            if (isCameraRotate)
            {
                // ��]���x���v�Z
                float speed = rotationSpeed * Time.deltaTime;
                // ��]�������]�t���O(isInversion)�������Ă���Ȃ�A���x�𔽓]
                if (isInversion)
                {
                    speed *= -1.0f;
                }

                // �N�_�̈ʒu�𒆐S�ɃJ��������]�ړ�������
                this.transform.RotateAround(Vector3.zero, Vector3.up, speed);
            }
        }

        /// <summary>
        /// �J�����ړ��J�n
        /// </summary>
        /// <param name="rightRotate">�E�����̈ړ��t���O�i�E�ړ����Ftrue�j</param>
        public void CameraRotateStart(bool rightRotate)
        {
            // �J������]�t���O��true��
            isCameraRotate = true;
            // ��]�������΃t���O��K�p����
            isInversion = rightRotate;
        }

        /// <summary>
        /// �J�����ړ��I��
        /// </summary>
        public void CameraRotateEnd()
        {
            // �J������]�t���O��false��
            isCameraRotate = false;
        }


        public void OverheadCamera()
        {
            //Debug.Log("OverheadCamera");

            //this.transform.position = targetTransform.position;
            //this.transform.rotation = targetTransform.rotation;

            var t = 1 - Mathf.Pow(0.1f, Time.deltaTime / 0.5f);

            this.transform.position =
                Vector3.Lerp(transform.position, targetTransform.position,t);
            this.transform.rotation =
                Quaternion.Lerp(transform.rotation, targetTransform.rotation, t);
        }

        public void ReturnCameraTransform()
        {
            //Debug.Log("ReturnCameraTransform");

            //this.transform.position = prevCameraTransform.position;
            //this.transform.rotation = prevCameraTransform.rotation;

            var t = 1 - Mathf.Pow(0.1f, Time.deltaTime / 0.5f);


            this.transform.position =
                Vector3.Lerp(transform.position, prevCameraTransform.position,t);
            this.transform.rotation =
                Quaternion.Lerp(transform.rotation, prevCameraTransform.rotation, t);
        }
    }
}
