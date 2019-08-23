using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static SceneRoot instance;

    public delegate void SceneRootCreatedEvent();

    public static SceneRootCreatedEvent SceneRootCreated;

    void Awake()
    {
        instance = this;
        SceneRootCreated?.Invoke();
    }
}
