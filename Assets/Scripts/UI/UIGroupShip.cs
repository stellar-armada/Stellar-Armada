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

        private ShipShield shield;
        private ShipHull hull;
        
        [SerializeField] private Color shieldColor;
        
        void Start()
        {
            if(ship == null) Debug.LogError("Ship was not initialized on UI ship");
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
            shieldImage.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, ( shieldVal / shield.maxShield)); // A UI shader controlling these variables would be cheaper

        }

        void HandleHullChange(float hullVal)
        {
            hullImage.fillAmount = hullVal / hull.maxHull; // A UI shader controlling these variables would be cheaper
        }
        
    }
}