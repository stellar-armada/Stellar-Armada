using StellarArmada.Levels;
using UnityEngine;

public class LockRotationToScene : MonoBehaviour
{
    private Transform t;
    private Transform rootTransform;
    void Start()
    {
        t = transform;
    }
    void LateUpdate()
    {
        if (rootTransform == null)
        {
            if(SceneRoot.instance != null) rootTransform = SceneRoot.instance.transform;
        }
        else t.rotation = rootTransform.rotation;
    }
}
