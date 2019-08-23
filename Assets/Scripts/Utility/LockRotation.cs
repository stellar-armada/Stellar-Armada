using StellarArmada.Scenarios;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Transform t;
    private Transform rootTransform;
    void Start()
    {
        t = transform;
        rootTransform = LevelRoot.instance.transform;
    }
    void LateUpdate()
    {
        if(rootTransform == null) rootTransform = LevelRoot.instance.transform;
        else t.rotation = rootTransform.rotation;
    }
}
