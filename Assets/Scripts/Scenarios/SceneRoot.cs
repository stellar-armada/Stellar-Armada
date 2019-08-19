using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static SceneRoot instance;

    void Awake()
    {
        instance = this;
    }
}
