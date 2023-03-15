using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock_New : MonoBehaviour
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

    public enum BlockAttribute
    {
        None,
        Trap,
        DamageBuff,
    }

    // �u���b�N�f�[�^
    // �C���X�y�N�^��Ŕ�\���ɂ���
    [HideInInspector]
    public int xPos;
    [HideInInspector]
    public int zPos;

    // �u���b�N�̑���
    public BlockAttribute blockAttribute;

    public bool isPassable;     // �ʍs�\���ǂ���

    // Start is called before the first frame update
    void Start()
    {
        // �q�̈�Ԗڂɂ���I�u�W�F�N�g���擾
        selectionObj = this.transform.GetChild(0).gameObject;

        // ������Ԃł͔�\��
        selectionObj.SetActive(false);

        // �u���b�N�̑����̏����ݒ�
        blockAttribute = BlockAttribute.None;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            SetSelectionBlockAttribute(BlockAttribute.None);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetSelectionBlockAttribute(BlockAttribute.DamageBuff);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetSelectionBlockAttribute(BlockAttribute.Trap);
        }
    }

    /// <summary>
    /// �����\���I�u�W�F�N�g�̕\���E��\����ݒ�
    /// </summary>
    /// <param name="isSelect">�I�����</param>
    public void SetSelectionMode(bool isSelect)
    {
        // �I�𒆂Ȃ�
        if (isSelect)
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
        switch (mode)
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

    /// <summary>
    /// �u���b�N�̑�����ݒ�
    /// </summary>
    /// <param name="attribute">�u���b�N�̑���</param>
    public void SetSelectionBlockAttribute(BlockAttribute attribute)
    {
        switch(attribute)
        {
            // �Ȃ�
            case BlockAttribute.None:
                blockAttribute = BlockAttribute.None;
                break;
            // �
            case BlockAttribute.Trap:
                blockAttribute = BlockAttribute.Trap;
                break;
            // �_���[�W�o�t
            case BlockAttribute.DamageBuff:
                blockAttribute = BlockAttribute.DamageBuff;
                break;
        }
    }
}
