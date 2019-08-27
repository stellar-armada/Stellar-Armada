using UnityEngine;

public class DefaultCamera : MonoBehaviour
{
    public static DefaultCamera instance;
    void Awake()
    {
        instance = this;
    }
}
