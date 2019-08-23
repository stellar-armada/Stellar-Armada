using UnityEngine;

public class PlacementCursor : MonoBehaviour
{
    public static PlacementCursor instance;

    public Transform transformToFollow;
    
    private Transform t;
    
    void Awake()
    {
        instance = this;
        t = GetComponent<Transform>();
    }
    void Start()
    {
        t.SetParent(MiniMap.instance.transform);
        t.localPosition = Vector3.zero;
    }

    void LateUpdate()
    {
        t.position = transformToFollow.position;
        t.rotation = transformToFollow.rotation;
    }
}
