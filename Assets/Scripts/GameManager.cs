using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager.MapManager mapManager;

        // Start is called before the first frame update
        void Start()
        {
            mapManager = GetComponent<MapManager.MapManager>();
        }

        // Update is called once per frame
        void Update()
        {
            // �^�b�v������o
            if(Input.GetMouseButtonDown(0))
            {
                GetMapObjects();
            }
        }

        /// <summary>
        /// �^�b�v�����ꏊ�̃I�u�W�F�N�g���擾
        /// </summary>
        private void GetMapObjects()
        {
            GameObject targetObject = null;     // �^�b�v�����I�u�W�F�N�g

            // �^�b�v���������ɃJ��������Ray���΂�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            // Ray�̓�����ʒu�ɑ��݂���I�u�W�F�N�g���擾
            if(Physics.Raycast(ray, out hit))
            {
                targetObject = hit.collider.gameObject;
            }

            // �ΏۃI�u�W�F�N�g�����݂���ꍇ�̏���
            if(targetObject != null)
            {
                SelectBlock(targetObject.GetComponent<MapBlock>());
            }
        }

        /// <summary>
        /// �w�肵���u���b�N��I����Ԃɂ���
        /// </summary>
        /// <param name="targetBlock">�Ώۂ̃I�u�W�F�N�g�f�[�^</param>
        private void SelectBlock(MapBlock targetObject)
        {
            // �S�u���b�N�̑I����Ԃ���������
            mapManager.AllSelectionModeClear();

            // �u���b�N��I����Ԃɂ���
            targetObject.SetSelectionMode(true);

            Debug.Log("�I�u�W�F�N�g���^�b�v����܂��� \n�u���b�N���W : "
                + targetObject.transform.position);
        }
    }

}