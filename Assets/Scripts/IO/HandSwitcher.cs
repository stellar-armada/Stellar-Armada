using SpaceCommander.IO;
using UnityEngine;

public class HandSwitcher : MonoBehaviour
{
    public static HandSwitcher instance;

    [SerializeField] Transform leftHandTarget = null;

    [SerializeField] Transform rightHandTarget = null;

    Transform currentTarget;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
        InputManager.instance.OnLeftTrigger += (on) =>
        {
            if(on) SwitchToLeftHand();
        };
        InputManager.instance.OnRightTrigger += (on) =>
        {
            if (on) SwitchToRightHand();
        };
    }

    public bool CurrentHandIsLeft()
    {
        return currentTarget == leftHandTarget;
    }

    public bool CurrentHandIsRight()
    {
        return currentTarget == rightHandTarget;
    }

    public void SwitchToRightHand()
    {
        transform.SetParent(rightHandTarget);
        currentTarget = rightHandTarget;
        ResetTransform();

    }

    public void SwitchToLeftHand()
    {
        transform.SetParent(leftHandTarget);
        currentTarget = leftHandTarget;
        ResetTransform();
    }

    void ResetTransform()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
