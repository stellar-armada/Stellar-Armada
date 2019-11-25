using StellarArmada.Levels;
using UnityEngine;

public class LockRotationToScene : MonoBehaviour
{
    private Transform t;
    private Transform rootTransform;

    public enum LockType
    {
        ToShip,
        ToRtsCamera
    }

    [SerializeField] private LockType lockType;
    
    void Start()
    {
        t = transform;
    }
    void LateUpdate()
    {
        switch (lockType)
        {
            case LockType.ToShip:
                if (rootTransform == null && LocalPlayerBridgeSceneRoot.instance != null)
                    rootTransform = LocalPlayerBridgeSceneRoot.instance.transform;
                else t.rotation = rootTransform.rotation;
                break;
            case LockType.ToRtsCamera:
                t.rotation = Quaternion.identity;
                break;
        }
    }
}
