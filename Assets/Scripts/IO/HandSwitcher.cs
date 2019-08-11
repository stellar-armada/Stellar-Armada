using UnityEngine;

public class HandSwitcher : MonoBehaviour
{

    [SerializeField] private Transform leftHandTarget;

    [SerializeField] private Transform rightHandTarget;

    private Transform t;
    void Awake()
    {
        t = transform;
        Deactivate();
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
        t.parent = rightHandTarget;
        ResetTransform();

    }

    public void SwitchToLeftHand()
    {
        Activate();
        transform.parent = leftHandTarget;
        ResetTransform();
    }

    void ResetTransform()
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }
}
