using UnityEngine;
using StellarArmada.Teams;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Manages the body of the player
    // TO-DO: Avatar management / customization and prefab instantiation
    public class BodyController : MonoBehaviour
    {
        [SerializeField] HumanPlayerController humanPlayerController; // Reference to the local player
        
        [SerializeField] SkinnedMeshRenderer[] bodyRenderers;
        public void Init()
        {
            humanPlayerController.EventOnPlayerTeamChange += HandleTeamChange;
        }

        public void HandleTeamChange()
        {
            uint pTeam = humanPlayerController.GetTeam().teamId;
            foreach (var ren in bodyRenderers)
            {
                ren.material.SetColor("_EmissionColor", TeamManager.instance.templates[pTeam].color);
            }

        }
        
    }
}