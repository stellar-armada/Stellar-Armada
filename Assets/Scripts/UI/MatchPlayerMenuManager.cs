using StellarArmada.Entities.Ships;
using StellarArmada.IO;
using StellarArmada.Levels;
using StellarArmada.Match;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Local player singleton
    // Manages the display of the match menu, located inside the MatchPlayer prefab
    public class MatchPlayerMenuManager : MonoBehaviour
    {
        public static MatchPlayerMenuManager instance;
        
        private Transform t;
        
        public bool menuIsActive;
        
        void Awake()
        {
            instance = this;
            t = transform;
        }
        void Start()
        {
            gameObject.SetActive(false);
        }

        [SerializeField] private Transform leftCanvasAttachPoint;
        [SerializeField] private Transform rightCanvasAttachPoint;

        public void AttachToLeftPoint()
        {
            t.SetParent(leftCanvasAttachPoint);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }


        public void AttachToRightPoint()
        {
            t.SetParent(rightCanvasAttachPoint);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }

        public void SetMenuState(bool state)
        {
            if (!MatchStateManager.instance.InMatch()) return;
            menuIsActive = state;
            // Trigger rollover
            if (state == false && Rollover.currentRollover != null)
            {
                Rollover.currentRollover.HandleRolloverReleasedOn();
            }

            if (state)
            {
                // Set parent to scene rootLookAtTransform
                t.SetParent(LocalPlayerBridgeSceneRoot.instance.transform, true);
                LookAt();
            }
            else
            {
                if(HandSwitcher.instance.CurrentHandIsRight()) AttachToRightPoint();
                else AttachToLeftPoint();
            }
            
            gameObject.SetActive(state);
        }

        void LookAt()
    {
        if (HandSwitcher.currentTarget == null) return;

        Transform b = LocalPlayerBridgeSceneRoot.instance.transform;
        
        // Get position of current target and transform into scene root's local space
        Vector3 currentTargetVector = b.InverseTransformPoint(HandSwitcher.currentTarget.position);
        
        // Get position of menu and do the same
        Vector3 menuVector = b.InverseTransformPoint(t.position);

        menuVector.y = currentTargetVector.y;

        Vector3 outVector =  menuVector - currentTargetVector;

        transform.localRotation = Quaternion.LookRotation(outVector, Vector3.up);
        
       
    }
    }
}