using UnityEngine;

public class EnvironmentTransformRoot : MonoBehaviour
{
    public static EnvironmentTransformRoot instance;

    void Awake()
    {
        instance = this;
    }
}
