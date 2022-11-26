using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionObj;    // 強調表示オブジェクト

    // ブロックデータ
    // インスペクタ上で非表示にする
    [HideInInspector]
    public int xPos;
    [HideInInspector]
    public int zPos;

    public bool isPassable;     // 通行可能かどうか

    // Start is called before the first frame update
    void Start()
    {
        // 子の一番目にあるオブジェクトを取得
        selectionObj = this.transform.GetChild(0).gameObject;

        // 初期状態では非表示
        selectionObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 強調表示オブジェクトの表示・非表示を設定
    /// </summary>
    /// <param name="isSelect">選択状態</param>
    public void SetSelectionMode(bool isSelect)
    {
        // 選択中なら
        if(isSelect)
        {
            selectionObj.SetActive(true);
        }
        // 選択中でなければ
        else
        {
            selectionObj.SetActive(false);
        }
    }
}
