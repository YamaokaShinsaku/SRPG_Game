using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionObj;    // 強調表示オブジェクト

    // 強調表示マテリアル
    public Material selectMaterial;
    public Material reachableMaterial;
    public Material attackableMaterial;

    // ブロックの強調表示モードを定義
    public enum Highlight
    {
        Off,        // オフ
        Select,     // 選択
        Reachable,  // 到達可能
        Attackable  // 攻撃可能
    }

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

    /// <summary>
    /// 選択状態オブジェクトの表示・非表示を設定
    /// </summary>
    /// <param name="mode">強調表示モード</param>
    public void SetSelectionMode(Highlight mode)
    {
        switch(mode)
        {
            // なし
            case Highlight.Off:
                selectionObj.SetActive(false);
                break;
            // 選択
            case Highlight.Select:
                selectionObj.GetComponent<Renderer>().material = selectMaterial;
                selectionObj.SetActive(true);
                break;
            // 到達可能
            case Highlight.Reachable:
                selectionObj.GetComponent<Renderer>().material = reachableMaterial;
                selectionObj.SetActive(true);
                break;
            // 攻撃可能
            case Highlight.Attackable:
                selectionObj.GetComponent<Renderer>().material = attackableMaterial;
                selectionObj.SetActive(true);
                break;
        }
    }
}
