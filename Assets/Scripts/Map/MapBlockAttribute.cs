using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlockAttribute : MonoBehaviour
{
    /// <summary>
    /// ブロックに持たせる属性
    /// </summary>
    public enum BlockAttribute
    {
        None,
        Trap,
        DamageBuff,
    }

    // ブロックの属性
    public BlockAttribute blockAttribute;

    // Start is called before the first frame update
    void Start()
    {
        // ブロックの属性の初期設定
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
    /// ブロックの属性を設定
    /// </summary>
    /// <param name="attribute">ブロックの属性</param>
    public void SetSelectionBlockAttribute(BlockAttribute attribute)
    {
        switch(attribute)
        {
            // なし
            case BlockAttribute.None:
                blockAttribute = BlockAttribute.None;
                break;
            // 罠
            case BlockAttribute.Trap:
                blockAttribute = BlockAttribute.Trap;
                break;
            // ダメージバフ
            case BlockAttribute.DamageBuff:
                blockAttribute = BlockAttribute.DamageBuff;
                break;
        }
    }
}
