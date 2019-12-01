using System.Linq;
using StellarArmada.Ships;
using StellarArmada.Player;
using StellarArmada.Teams;
using TMPro;
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

        [SerializeField] private GameObject shipyardDescriptionPanel;
        
        [SerializeField] CanvasGroup canvasGroup;

        private Team team;

        private bool isInteractable = true;

        [SerializeField] TextMeshProUGUI PriceText;
        
        void Start()
        {
            team = PlayerController.localPlayer.GetTeam();
            // If the id is -1 show the description
            shipyardDescriptionPanel.gameObject.SetActive(id == -1);
        }

        public void SetInteractable(bool val)
        {
            isInteractable = val;
            if (isInteractable)
            {
                canvasGroup.alpha = 1.0f;
                canvasGroup.blocksRaycasts = true;
                PriceText.color = Color.white;
            }
            else
            {
                PriceText.color = Color.red;
                canvasGroup.alpha = .5f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public bool IsInteractable() => isInteractable;

        public void ShowShipyardDescriptionPanel()
        {
            shipyardDescriptionPanel.gameObject.SetActive(true);
        }
        public void HideShipyardDescriptionPanel()
        {
            shipyardDescriptionPanel.gameObject.SetActive(false);
        }
        public void ResetShipyardDescriptionPanel()
        {
            shipyardDescriptionPanel.gameObject.SetActive(id == -1);
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
            {
                SetFlagToActiveForLocalUser();
                FlagshipPortraitController.instance.SetFlagship(shipType);
            }
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