using UnityEngine;
using Mirror;
using StellarArmada.Game;
using StellarArmada.Players;
using StellarArmada.Teams;
using StellarArmada.UI;

public enum ShipMeta
{
    None,
    Flagship_Empty,
    Flagship_Occupied
}

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class Ship : NetworkEntity
    {
        [HideInInspector] public PlayerController playerController;

        public enum FormationPosition
        {
            Frontline = 0,
            Midline = 1,
            Backline = 2
        }
        
        public ShipType type;
        public bool availableAsFlagship; // can be start on and be switched off if a player populates this ship
        
        public FormationPosition formationPosition;

        [Header("Ship Subsystems")]
        public ShipBridge bridge;
        public Hull hull;
        public Shield shield;
        public ShipWeaponSystemController weaponSystemController;
        public ShipWarp shipWarp;
        public StatusBar statusBar;

        [Header("Ship Components")]
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

        public override void Die()
        {
            base.Die();
            statusBar.FadeOutStatusBar();
            entityExplosion.Explode();
            weaponSystemController.weaponSystemsEnabled = false;
            weaponSystemController.HideWeaponSystems();
            shield.currentShield = 0;
            shield.gameObject.SetActive(false);
            if(HumanPlayerController.localPlayer != null == team.players.Contains(HumanPlayerController.localPlayer))
                SelectionUIManager.instance.RemoveSelectableFromSelectionSets(entityId);
        }

    }
}