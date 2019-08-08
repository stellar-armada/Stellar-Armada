using UnityEngine;
using SpaceCommander.Teams;

namespace SpaceCommander.Player
{
    
    // TO-DO: Separate IK    
    
    
    /* Manages visibility of the player's body
     * Player body is hidden from local player
     * To everyone else, player materializes on spawn and dematerializes on death
     * If you want to swap your player mesh based on class, do that here
     * */
    public class BodyController : MonoBehaviour
    {
        // [SerializeField] private VRIK vrik;

        [SerializeField] PlayerController playerController; // Reference to the local player

        [SerializeField] private Transform controllerRoot;

        [SerializeField] SkinnedMeshRenderer bodyRenderer;
        [SerializeField] Transform mainHandAnchor;
        
        // These are IK anchor points. Requires Final IK
        /*
        [SerializeField] Transform mobileWeaponAttachPointLeft;
        [SerializeField] Transform mobileWeaponAttachPointRight;
        [SerializeField] Transform mobileHandAttachPointLeft;
        [SerializeField] Transform mobileHandAttachPointRight;

        [SerializeField] Transform offIKHandAnchorLeft; // Follower hand when main hand is right
        [SerializeField] Transform offIKHandAnchorRight; // Folower hand when main hand is left
        [SerializeField] Transform leftHandIKAnchor; // Main hand anchor when main hand is left
        [SerializeField] Transform rightHandIKAnchor; // Main hand anchor when main hand is right
        */

        private MaterialPropertyBlock props;
        public void Init()
        {
            props = new MaterialPropertyBlock();
            playerController.EventOnWeaponHandChange += HandleWeaponHandChange;
            playerController.EventOnPlayerTeamChange += HandleTeamChange;
        }

        public void EnableRightHandIK()
        {
            // vrik.solver.rightArm.positionWeight = 1;
            // vrik.solver.rightArm.rotationWeight = 1;
            // vrik.solver.rightArm.shoulderTwistWeight = 1;
            // vrik.solver.rightArm.shoulderRotationWeight = 1;
        }

        public void EnableLeftHandIK()
        {
            // vrik.solver.leftArm.positionWeight = 1;
            // vrik.solver.leftArm.rotationWeight = 1;
            // vrik.solver.leftArm.shoulderTwistWeight = 1;
            // vrik.solver.leftArm.shoulderRotationWeight = 1;
        }

        public void DisableLeftHandIK()
        {
            // vrik.solver.leftArm.positionWeight = 0;
            // vrik.solver.leftArm.rotationWeight = 0;
            // vrik.solver.leftArm.shoulderTwistWeight = 0;
            // vrik.solver.leftArm.shoulderRotationWeight = 0;
        }

        public void DisableRightHandIK()
        {
            // vrik.solver.rightArm.positionWeight = 0;
            // vrik.solver.rightArm.rotationWeight = 0;
            // vrik.solver.rightArm.shoulderTwistWeight = 0;
            // vrik.solver.rightArm.shoulderRotationWeight = 0;
        }

        public void HandleTeamChange()
        {
            uint pTeam = playerController.GetTeam().teamId;
            props.SetColor("_EmissionColor", TeamManager.instance.templates[pTeam].color);
            bodyRenderer.SetPropertyBlock(props);
        }

        public void HandleWeaponHandChange()
        {
            /*
            if (IPlayer.playerWeaponHand == GameManager.PlayerWeaponHand.Left)
            {
                EnableLeftHandIK();
                DisableRightHandIK();
                // set left hand to main anchor
                leftHandIKAnchor.parent = mainHandAnchor;
                leftHandIKAnchor.localPosition = Vector3.zero;
                leftHandIKAnchor.localRotation = Quaternion.identity;

                // set right hand to right offset anchor
                rightHandIKAnchor.parent = offIKHandAnchorRight;
                rightHandIKAnchor.localPosition = Vector3.zero;
                rightHandIKAnchor.localRotation = Quaternion.identity;
            }
            else
            {
                DisableLeftHandIK();
                EnableRightHandIK();
                // set right hand to right offset anchor
                rightHandIKAnchor.parent = mainHandAnchor;
                rightHandIKAnchor.localPosition = Vector3.zero;
                rightHandIKAnchor.localRotation = Quaternion.identity;

                // set left hand to main anchor
                leftHandIKAnchor.parent = offIKHandAnchorLeft;
                leftHandIKAnchor.localPosition = Vector3.zero;
                leftHandIKAnchor.localRotation = Quaternion.identity;
            }
            */
        }
    }
}