using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPoint : MonoBehaviour
{
    public Vector3 moveOffset;

    public Transform target;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 targetPos = new Vector3();
            targetPos = target.TransformPoint(moveOffset);
            Debug.Log(target.position);
            Debug.Log(targetPos);
        }
    }
}
