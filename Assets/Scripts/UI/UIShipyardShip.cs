using System;
using System.Linq;
using StellarArmada.Entities.Ships;
using StellarArmada.Player;
using StellarArmada.Teams;
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
        public int id = -1;

        public ShipType shipType;
        
        [SerializeField] Image flag;

        private Team team;

        void Start()
        {
            team = PlayerController.localPlayer.GetTeam();

        }

        public ShipPrototype GetPrototype()
        {
            return team.prototypes.Single(p => p.id == id);
        }

        public void SetPrototype(ShipPrototype prototype)
        {
            team.prototypes[team.prototypes.IndexOf(team.prototypes.First(p => p.id == prototype.id))] = prototype;
        }

        public void SetFlagshipStatus()
        {
            var p = GetPrototype();
            if (p.hasCaptain)
            {
            uint captain = GetPrototype().captain;
            if (captain == PlayerController.localPlayer.netId)
                SetFlagToActiveForLocalUser();
            else
                SetFlagToActiveForTeamMate();
            }
            else
            {
                SetFlagToInactive();
            }
        }

        void SetFlagToActiveForLocalUser()
        {
            flag.color = Color.green;
        }

        void SetFlagToActiveForTeamMate()
        {
            // TO-DO: Add teammate notify of occupation of ship
            flag.color = Color.blue;
        }

        void SetFlagToInactive()
        {
            flag.color = new Color(0,0,0,0);
        }
    }
}