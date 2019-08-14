using SpaceCommander.Ships;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
namespace SpaceCommander.UI
{
    public class UIShip : MonoBehaviour
    {
        public Ship ship;
        public Image shieldImage;
        public Image hullImage;

        private ShipShield shield;
        private ShipHull hull;
        
        void Start()
        {
            shield = ship.shipShield;
            hull = ship.shipHull;
            shield.ShieldChanged.AddListener(HandleShieldChange);
            hull.HullChanged.AddListener(HandleHullChange);
        }

        void HandleShieldChange()
        {
            shieldImage.fillAmount = shield.currentShield / shield.maxShield; // TO-DO
        }

        void HandleHullChange()
        {
            hullImage.fillAmount = hull.currentHull / hull.maxHull; // A UI shader controlling these variables would be cheaper
        }
        
    }
}