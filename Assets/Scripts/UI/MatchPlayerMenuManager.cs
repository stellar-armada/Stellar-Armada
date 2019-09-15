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
   
    
    Vector3 LookatXZ(Transform lookat)
    {
    Vector3 distance = lookat.position - t.position;
    Vector3 direction = Vector3.ProjectOnPlane(distance, t.up).normalized;
        return t.InverseTransformVector(direction);
    }
    
    private static float AngleXZ(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    void LookAt()
    {
        t.localRotation = Quaternion.Euler(Vector3.up * AngleXZ(LookatXZ(HandSwitcher.currentTarget)));
    }
    
    void LateUpdate()
    {
        LookAt();
    }
    }
}