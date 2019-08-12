using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Transform t;

    void Awake()
    {
        t = transform;
    }
    void LateUpdate()
    {
        t.rotation = Quaternion.identity;
    }
}
