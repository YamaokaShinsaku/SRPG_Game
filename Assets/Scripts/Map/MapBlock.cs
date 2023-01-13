using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionObj;    // �����\���I�u�W�F�N�g

    // �����\���}�e���A��
    public Material selectMaterial;
    public Material reachableMaterial;
    public Material attackableMaterial;

    // �u���b�N�̋����\�����[�h���`
    public enum Highlight
    {
        Off,        // �I�t
        Select,     // �I��
        Reachable,  // ���B�\
        Attackable  // �U���\
    }

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

    /// <summary>
    /// �I����ԃI�u�W�F�N�g�̕\���E��\����ݒ�
    /// </summary>
    /// <param name="mode">�����\�����[�h</param>
    public void SetSelectionMode(Highlight mode)
    {
        switch(mode)
        {
            // �Ȃ�
            case Highlight.Off:
                selectionObj.SetActive(false);
                break;
            // �I��
            case Highlight.Select:
                selectionObj.GetComponent<Renderer>().material = selectMaterial;
                selectionObj.SetActive(true);
                break;
            // ���B�\
            case Highlight.Reachable:
                selectionObj.GetComponent<Renderer>().material = reachableMaterial;
                selectionObj.SetActive(true);
                break;
            // �U���\
            case Highlight.Attackable:
                selectionObj.GetComponent<Renderer>().material = attackableMaterial;
                selectionObj.SetActive(true);
                break;
        }
    }
}
