using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;    // �Ǐ]����I�u�W�F�N�g
    [SerializeField]
    private APBattleManager apbattleManager;    // ���ݑI������Ă���L�����N�^�[���擾

    public Vector3 offset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (apbattleManager.selectingCharacter)
        {
            this.transform.position = apbattleManager.selectingCharacter.transform.position + offset;
        }
        else
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray();
            RaycastHit hit = new RaycastHit();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //�}�E�X�N���b�N�����ꏊ����Ray���΂��A�I�u�W�F�N�g�������true 
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "DirectionObj")
                {
                    Debug.Log("Direction Hit");
                }
            }
        }
    }


}
