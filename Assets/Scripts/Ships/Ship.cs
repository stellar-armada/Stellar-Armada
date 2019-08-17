using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Players;
using SpaceCommander.Teams;
using SpaceCommander.UI;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class Ship : NetworkEntity
    {
        public PlayerController playerController;

        public ShipType type;
        [Header("Ship Subsystems")]
        public EntityMovement movement;
        public Hull hull;
        public Shield shield;
        public ShipWeaponSystemController weaponSystemController;
        public ShipExplosion shipExplosion;
        public ShipWarp shipWarp;
        public StatusBar statusBar;

        [Header("Ship Components")]
        public Collider shipCollider;
        public Renderer visualModel;
      
        [Command]
        public void CmdSetPlayer(uint playerId)
        {
            playerController = PlayerManager.GetPlayerById(playerId);
        }

        public PlayerController GetPlayer() => playerController;

        void SetGroup(int newGroupId)
        {
            team.ChangeEntityGroup(this, newGroupId);
        }
        
        [Command]
        public override void CmdSetTeam(uint newTeamId)
        {
            SetTeam(newTeamId);
            RpcSetTeam(newTeamId);
        }

        [ClientRpc]
        public void RpcSetTeam(uint newTeamId)
        {
            if(!isServer) // Prevent double calls if we're testing in host mode
            SetTeam(newTeamId);
        }

        void SetTeam(uint newTeamId)
        {
            if (team != null && team.entities.Contains(this) && team.teamId != newTeamId) team.RemoveEntity(this);
            team = TeamManager.instance.GetTeamByID(newTeamId);
            team.AddEntity(this);
            statusBar.SetInsignia(team.insignia);
        }
        
        
        [Command]
        public void CmdActivate()
        {
            isAlive = true;
            statusBar.ShowStatusBar(); 
            weaponSystemController.weaponSystemsEnabled = true;
        }
        
        [Command]
        public override void CmdDie()
        {
            base.CmdDie();
            statusBar.FadeOutStatusBar();
            shipExplosion.Explode();
            weaponSystemController.weaponSystemsEnabled = false;
            weaponSystemController.HideWeaponSystems();
            shield.currentShield = 0;
            shield.gameObject.SetActive(false);

            // Foreach player, tell their selection manager to clear
            foreach (PlayerController player in team.players)
            {
                if (player.isServer)
                {
                    RemoveFromSelectionSets(entityId); // If we're host, let's just do it here
                }
                else
                {
                    TargetRemoveFromSelectionSets(player.connectionToClient, entityId); // Otherwise, tell clients on the team to remove the ship
                }
            }
        }
        
        [TargetRpc]
        void TargetRemoveFromSelectionSets(NetworkConnection target, uint entityId)
        {
            RemoveFromSelectionSets(entityId);
        }

        void RemoveFromSelectionSets(uint shipId)
        {
            if (HumanPlayerController.localPlayer == null) return; // No local human player, so don't worry about selection
            SelectionUIManager.instance.RemoveSelectableFromSelectionSets(entityId);
        }
    }
}