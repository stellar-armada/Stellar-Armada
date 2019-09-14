using StellarArmada.IO;
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
            SetMenuState(false);
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
            // Trigger rollover
            if (state == false && Rollover.currentRollover != null)
            {
                Rollover.currentRollover.HandleRolloverReleasedOn();
            }
            
            gameObject.SetActive(state);
        }
    }
}