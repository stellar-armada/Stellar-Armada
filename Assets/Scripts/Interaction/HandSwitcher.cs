using StellarArmada.IO;
using UnityEngine;

// Script for local player to switch dominant hand
// By default, when trigger is called, if the trigger hand isn't dom
public class HandSwitcher : MonoBehaviour
{
    public static HandSwitcher instance; // Singleton reference, since this is a local player script

    // Attachment points
    [SerializeField] Transform leftHandTarget = null;
    [SerializeField] Transform rightHandTarget = null;

    // Current attachment point
    Transform currentTarget;
    
    // Local var ref
    private Transform t;
    
    void Awake()
    {
        instance = this; // Singleton ref
        t = GetComponent<Transform>();
    }
    
    void Start()
    {
        // Anonymous subsriptions to filter button-up event
        InputManager.instance.OnLeftTrigger += (on) =>
        {
            if(on) SetHand(leftHandTarget);
        };
        
        InputManager.instance.OnRightTrigger += (on) =>
        {
            if (on) SetHand(rightHandTarget);
        };
    }
    
    public void SetHand(Transform handTarget)
    {
        t.SetParent(handTarget);
        currentTarget = handTarget;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

    // Helper functions for querying state
    public bool CurrentHandIsLeft() => currentTarget == leftHandTarget;
    public bool CurrentHandIsRight() => currentTarget == rightHandTarget;


}
