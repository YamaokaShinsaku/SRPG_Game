using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class DirectionUIManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;        // UI�������Ă���Canvas
        [SerializeField]
        private Transform targetTransform;    // �Ǐ]����I�u�W�F�N�g
        [SerializeField]
        private APBattleManager apbattleManager;    // ���ݑI������Ă���L�����N�^�[���擾

        // RectTransform���擾���邽�߂̕ϐ�
        private RectTransform canvasRectTransform;
        private RectTransform myRectTransform;

        // UI�摜�̃I�t�Z�b�g�ݒ�p
        public Vector3 offset = new Vector3(0, 0, 0);

        // Start is called before the first frame update
        void Start()
        {
            // �R���|�[�l���g���擾
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            myRectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            // UI�摜�̍��W�ݒ�p�ϐ�
            Vector2 pos;
            if(apbattleManager.selectingCharacter)
            {
                targetTransform = apbattleManager.selectingCharacter.transform;
            }
            else
            {
                return;
            }

            // Canvas��RenderMode�ɉ�����UI�̍��W�̒������s��
            switch (canvas.renderMode)
            {

                case RenderMode.ScreenSpaceOverlay:
                    myRectTransform.position =
                        RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position + offset);

                    break;

                case RenderMode.ScreenSpaceCamera:
                    Vector2 screenPos =
                        RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position + offset);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRectTransform, screenPos, Camera.main, out pos);
                    myRectTransform.localPosition = pos;
                    break;

                case RenderMode.WorldSpace:
                    myRectTransform.LookAt(Camera.main.transform);

                    break;
            }
        }
    }

}