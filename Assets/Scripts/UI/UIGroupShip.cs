using StellarArmada.Entities.Ships;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Local player representation of ships in their fleet
    // Instantiated by the UI Ship Factory
    // Interacted with in the player's match menu
    public class UIGroupShip : MonoBehaviour
    {
        public Ship ship;
        public Image shieldImage;
        public Image hullImage;
        [SerializeField] GameObject selectionIndicator;

        [SerializeField] private Color shieldColor;
        private CanvasGroup group;

        void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }
        
        public void Start()
        {
            Transform t = transform;
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            ship.shield.ShieldChanged += HandleShieldChange;
            ship.hull.HullChanged += HandleHullChange;
            ship.shipSelectionHandler.OnSelectionChanged += HandleSelectionChange;
            selectionIndicator.SetActive(false);
            DisableControl();
        }

        public void EnableControl()
        {
            group.blocksRaycasts = true;
            group.interactable = true;
        }

        public void DisableControl()
        {
            group.blocksRaycasts = false;
            group.interactable = false;
        }

        void HandleSelectionChange(bool on)
        {
            selectionIndicator.SetActive(on);
        }

        void HandleShieldChange(float shieldVal)
        {
            shieldImage.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, ( shieldVal / ship.shield.maxShield)); // A UI shader controlling these variables would be cheaper
        }

        void HandleHullChange(float hullVal)
        {
            hullImage.fillAmount = hullVal / ship.hull.maxHull; // A UI shader controlling these variables would be cheaper
        }
        
    }
}