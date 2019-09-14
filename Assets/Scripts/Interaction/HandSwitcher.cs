using StellarArmada.IO;
using StellarArmada.UI;
using UnityEngine;

// Script for local player to switch dominant hand
// By default, when trigger is called, if the trigger hand isn't dom
public class HandSwitcher : MonoBehaviour
{
    public static HandSwitcher instance; // Singleton reference, since this is a local player script

    // Attachment points
    [SerializeField] Transform leftHandTarget = null;
    [SerializeField] Transform rightHandTarget = null;

    public enum CurrentHand
    {
        Right,
        Left
    }

    private CurrentHand currentHand = CurrentHand.Right;

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
            if(on) SetHand(CurrentHand.Left);
        };
        
        InputManager.instance.OnRightTrigger += (on) =>
        {
            if (on) SetHand(CurrentHand.Right);
        };
    }
    
    public void SetHand(CurrentHand hand)
    {
        Transform handTarget = null;
        switch (hand)
        {
            case CurrentHand.Left:
                handTarget = leftHandTarget;
                MatchPlayerMenuManager.instance.AttachToLeftPoint();
                break;
            case CurrentHand.Right:
                handTarget = rightHandTarget;
                MatchPlayerMenuManager.instance.AttachToRightPoint();
                break;
        }
        
        t.SetParent(handTarget);
        currentTarget = handTarget;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

    // Helper functions for querying state
    public bool CurrentHandIsLeft() => currentHand == CurrentHand.Left;
    public bool CurrentHandIsRight() => currentHand == CurrentHand.Right;


}
