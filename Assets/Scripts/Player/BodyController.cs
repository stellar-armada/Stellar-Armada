using UnityEngine;
using SpaceCommander.Teams;

#pragma warning disable 0649
namespace SpaceCommander.Players
{
    public class BodyController : MonoBehaviour
    {
        [SerializeField] HumanPlayerController humanPlayerController; // Reference to the local player
        
        [SerializeField] SkinnedMeshRenderer[] bodyRenderers;
        
        private MaterialPropertyBlock props;
        
        public void Init()
        {
            props = new MaterialPropertyBlock();
            humanPlayerController.EventOnPlayerTeamChange += HandleTeamChange;
        }

        public void HandleTeamChange()
        {
            uint pTeam = humanPlayerController.GetTeam().teamId;
            foreach (var ren in bodyRenderers)
            {
                ren.GetPropertyBlock(props);
                props.SetColor("_EmissionColor", TeamManager.instance.templates[pTeam].color);
                ren.SetPropertyBlock(props);
            }

        }
        
    }
}