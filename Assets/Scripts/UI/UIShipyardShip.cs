using System;
using StellarArmada.Entities.Ships;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Local player representation of ships in their fleet
    // Instantiated by the UI Ship Factory
    // Interacted with in the player's match menu
    public class UIShipyardShip : MonoBehaviour
    {
        public ShipPrototype prototype;
        [SerializeField] Image flag;
        private void Awake()
        {
            SetFlagToInactive();
        }

        void Start()
        {
            Shipyard.instance.PlaceShipInGroup(this, group);
        }

        public void SetAsFlagship()
        {
            Debug.Log("Setting flagship for local player -- " + gameObject.name);
            Shipyard.instance.SetFlagshipForLocalPlayer(this);
            SetFlagToActiveForLocalUser();
        }

        public void SetFlagToActiveForLocalUser()
        {
            flag.color = Color.green;
            hasCaptain = true;
        }

        public void SetFlagToActiveForTeamMate()
        {
            // TO-DO: Add teammate notify of occupation of ship
            flag.color = Color.blue;
            hasCaptain = true;
        }

        public void SetFlagToInactive()
        {
            flag.color = Color.gray;
        }
    }
}