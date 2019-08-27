using UnityEngine;

#pragma warning disable 0649
public class FollowLocalTransform : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool rot;
    
    [SerializeField] private bool pos;

    private Transform t;

    void Awake()
    {
        t = transform;
    }
    void LateUpdate()
    {
        if (rot) t.localRotation = target.localRotation;
        if (pos) t.localPosition = target.localPosition;
    }
}
