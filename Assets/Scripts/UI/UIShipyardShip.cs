using StellarArmada.Entities.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Local player representation of ships in their fleet
    // Instantiated by the UI Ship Factory
    // Interacted with in the player's match menu
    public class UIShipyardShip : MonoBehaviour
    {
        public ShipType shipType;
        public int group;

        void Start()
        {
            Shipyard.instance.PlaceShipInGroup(this, group);
        }
    }
}