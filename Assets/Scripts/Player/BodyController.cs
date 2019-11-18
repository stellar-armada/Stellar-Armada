using UnityEngine;
using StellarArmada.Teams;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Manages the body of the player
    // TO-DO: Avatar management / customization and prefab instantiation
    public class BodyController : MonoBehaviour
    {
        [FormerlySerializedAs("humanPlayerController")] [SerializeField] PlayerController playerController; // Reference to the local player
        
        [SerializeField] SkinnedMeshRenderer[] bodyRenderers;
        public void Init()
        {
            playerController.EventOnPlayerTeamChange += HandleTeamChange;
        }

        public void HandleTeamChange()
        {
            uint pTeam = playerController.GetTeam().teamId;
            foreach (var ren in bodyRenderers)
            {
                ren.material.SetColor("_EmissionColor", TeamManager.instance.templates[pTeam].color);
            }

        }
        
    }
}