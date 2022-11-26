using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionObj;    // �����\���I�u�W�F�N�g

    // �u���b�N�f�[�^
    // �C���X�y�N�^��Ŕ�\���ɂ���
    [HideInInspector]
    public int xPos;
    [HideInInspector]
    public int zPos;

    public bool isPassable;     // �ʍs�\���ǂ���

    // Start is called before the first frame update
    void Start()
    {
        // �q�̈�Ԗڂɂ���I�u�W�F�N�g���擾
        selectionObj = this.transform.GetChild(0).gameObject;

        // ������Ԃł͔�\��
        selectionObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �����\���I�u�W�F�N�g�̕\���E��\����ݒ�
    /// </summary>
    /// <param name="isSelect">�I�����</param>
    public void SetSelectionMode(bool isSelect)
    {
        // �I�𒆂Ȃ�
        if(isSelect)
        {
            selectionObj.SetActive(true);
        }
        // �I�𒆂łȂ����
        else
        {
            selectionObj.SetActive(false);
        }
    }
}
