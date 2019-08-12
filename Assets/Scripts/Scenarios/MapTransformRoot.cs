using UnityEngine;

public class MapTransformRoot : MonoBehaviour
{
    public static MapTransformRoot instance;

    void Awake()
    {
        instance = this;
    }
}
