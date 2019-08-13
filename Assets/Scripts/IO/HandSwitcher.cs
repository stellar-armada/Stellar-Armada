using UnityEngine;

public class HandSwitcher : MonoBehaviour
{
    public static HandSwitcher instance;

    [SerializeField] Transform leftHandTarget;

    [SerializeField] Transform rightHandTarget;

    Transform currentTarget;

    void Awake()
    {
        instance = this;
        Deactivate();
    }

    public bool CurrentHandIsLeft()
    {
        Debug.Log("currentTarget: " + currentTarget);
        return currentTarget == leftHandTarget;
    }

    public bool CurrentHandIsRight()
    {
        Debug.Log("currentTarget: " + currentTarget);

        return currentTarget == rightHandTarget;
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void SwitchToRightHand()
    {
        Activate();
        transform.SetParent(rightHandTarget);
        currentTarget = rightHandTarget;
        ResetTransform();

    }

    public void SwitchToLeftHand()
    {
        Activate();
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
