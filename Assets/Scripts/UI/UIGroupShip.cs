using SpaceCommander.Ships;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
namespace SpaceCommander.UI
{
    public class UIGroupShip : MonoBehaviour
    {
        public Ship ship;
        public Image shieldImage;
        public Image hullImage;
        [SerializeField] GameObject selectionIndicator;

        [SerializeField] private Color shieldColor;

        public void Start()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            ship.shipShield.ShieldChanged += HandleShieldChange;
            ship.shipHull.HullChanged += HandleHullChange;
            ship.selectionHandler.OnSelectionChanged += HandleSelectionChange;
        }

        void HandleSelectionChange(bool on)
        {
            selectionIndicator.SetActive(on);
        }

        void HandleShieldChange(float shieldVal)
        {
            shieldImage.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, ( shieldVal / ship.shipShield.maxShield)); // A UI shader controlling these variables would be cheaper

        }

        void HandleHullChange(float hullVal)
        {
            hullImage.fillAmount = hullVal / ship.shipHull.maxHull; // A UI shader controlling these variables would be cheaper
        }
        
    }
}