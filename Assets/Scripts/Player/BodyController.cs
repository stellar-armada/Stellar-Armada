using UnityEngine;
using SpaceCommander.Teams;

namespace SpaceCommander.Player
{

    /* Manages visibility of the player's body
     * Player body is hidden from local player
     * To everyone else, player materializes on spawn and dematerializes on death
     * If you want to swap your player mesh based on class, do that here
     * */
    public class BodyController : MonoBehaviour
    {
        [SerializeField] PlayerController playerController; // Reference to the local player
        
        [SerializeField] SkinnedMeshRenderer[] bodyRenderers;
        
        private MaterialPropertyBlock props;
        
        public void Init()
        {
            props = new MaterialPropertyBlock();
            playerController.EventOnPlayerTeamChange += HandleTeamChange;
        }

        public void HandleTeamChange()
        {
            uint pTeam = playerController.GetTeam().teamId;
            foreach (var ren in bodyRenderers)
            {
                ren.GetPropertyBlock(props);
                props.SetColor("_EmissionColor", TeamManager.instance.templates[pTeam].color);
                ren.SetPropertyBlock(props);
            }

        }
        
    }
}