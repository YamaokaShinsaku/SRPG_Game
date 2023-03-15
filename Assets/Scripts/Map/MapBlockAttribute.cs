using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlockAttribute : MonoBehaviour
{
    /// <summary>
    /// �u���b�N�Ɏ������鑮��
    /// </summary>
    public enum BlockAttribute
    {
        None,
        Trap,
        DamageBuff,
    }

    // �u���b�N�̑���
    public BlockAttribute blockAttribute;

    // Start is called before the first frame update
    void Start()
    {
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
