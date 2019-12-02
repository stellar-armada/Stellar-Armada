using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlane : MonoBehaviour
{
    private Transform parent;

    void Awake()
    {
        parent = transform.parent;
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(parent.position.x, 0, parent.position.z);
        transform.rotation = Quaternion.identity;
    }
}
