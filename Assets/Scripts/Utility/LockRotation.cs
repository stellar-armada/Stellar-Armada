using SpaceCommander.Scenarios;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Transform t;
    private Transform rootTransform;
    void Awake()
    {
        t = transform;
        rootTransform = SceneTransformableParent.instance.transform;
    }
    void LateUpdate()
    {
        // Remember, we are probably rotating ourselves (the map is static in the scene in inverse scale mode, set in MapControls
        t.rotation = rootTransform.rotation;
    }
}
