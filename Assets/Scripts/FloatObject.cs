using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject : MonoBehaviour
{
    public float nowPosi;

    void Start()
    {
        nowPosi = this.transform.position.y;
    }

    void Update()
    {
        transform.position =
            new Vector3(transform.position.x, nowPosi + Mathf.PingPong(Time.time / 3, 0.3f), transform.position.z);
    }
}
