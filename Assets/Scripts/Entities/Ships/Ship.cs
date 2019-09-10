using System;
using UnityEngine;
using Mirror;
using StellarArmada.Player;
using StellarArmada.Teams;

// In the current iteration, ships are organized into three formation roles
// Ships will be stacked in a grid adjacent to other ships in their line
public enum FormationPosition
{
    Frontline = 0,
    Midline = 1,
    Backline = 2
}

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    // Ships are a special type of network entity that can be controlled by a commander
    // Ships are owned by teams, but may have a captain
    public class Ship : NetworkEntity
    {
        // For this implementation, ships can be captained by players
        // Captains are placed on the bridge, which is instantiated for all captained ships
        public PlayerController captain;

        // An enum identifying what type of ship this is
        public ShipType type;

        // handy reference to which group this ship is in
        public int group = -1;

        // Where in the formation should this ship be? Frontline, backline, etc.
        public FormationPosition formationPosition;

        [Header("Ship Subsystems")] public ShipBridge bridge; //Instantiated and set when the captain choose this ship

        public ShipWarp
            shipWarp; // Manages the visual ship warp, as well as enabling the ship for command when warped in

        public MiniMapStatusBar miniMapStatusBar; // Insignia, health/shields and captain nameplate in the minimap
        public ShipSelectionHandler shipSelectionHandler; // Handles all logic related to local player selection

        public Action OnCaptainUpdated = delegate { }; // Delegate called when a captain is set for the ship

        [Command] // Server-side logic
        public void CmdSetCaptain(uint playerId)
        {
            Debug.Log("<color=red>CAPTAIN</color> Captain set to " + playerId + " on server");
            captain = PlayerManager.GetPlayerById(playerId);
            OnCaptainUpdated?.Invoke();
            RpcSetCaptain(playerId);
        }

        [ClientRpc] // Client-side logic
        public void RpcSetCaptain(uint playerId)
        {
            if (!isServer)
            {
                // Prevent double calls on host
                Debug.Log("<color=red>CAPTAIN</color> Captain set to " + playerId + " on client");
                captain = PlayerManager.GetPlayerById(playerId);
                OnCaptainUpdated?.Invoke();
            

            // Local player logic
            if (captain == HumanPlayerController.localPlayer)
            {
                Debug.Log("<color=red>CAPTAIN</color> Picking capital ship for local player " + captain.netId);
                ((HumanPlayerController) captain).PickCapitalShip(this);
                bridge.ActivateBridgeForLocalPlayer();
            }
            }
        }

        public PlayerController GetCaptain() => captain;

        // Change battlegroup for ship for local player
        void SetGroup(int newGroupId)
        {
            team.ChangeEntityGroup(this, newGroupId);
        }

        [Command] // Server logic
        public override void CmdSetTeam(uint newTeamId)
        {
            SetTeam(newTeamId);
            RpcSetTeam(newTeamId);
        }

        [ClientRpc] // Client logic
        public void RpcSetTeam(uint newTeamId)
        {
            if (!isServer) // Prevent double calls if we're testing in host mode
                SetTeam(newTeamId);
        }

        void SetTeam(uint newTeamId) // Called on both server and client
        {
            if (team != null && team.entities.Contains(this) && team.teamId != newTeamId) team.RemoveEntity(this);
            team = TeamManager.instance.GetTeamByID(newTeamId);
            team.AddEntity(this);
            miniMapStatusBar.SetInsignia(team.insignia);
        }

        // Call this to kill a ship -- should only ever be called server side, since nobody has local authority over entities
        [Command]
        public override void CmdDie()
        {
            Die();
            RpcDie();
        }

        [ClientRpc]
        public override void RpcDie()
        {
            Die();
        }

        void Die()
        {
            // TO DO -- move these all to delegate subscriptions instead of direct calls
            isAlive = false;
            miniMapStatusBar.FadeOutStatusBar();
            entityExplosion.Explode();
            weaponSystemController.weaponSystemsEnabled = false;
            weaponSystemController.HideWeaponSystems();
            shield.currentShield = 0;
            shield.gameObject.SetActive(false);
            if (HumanPlayerController.localPlayer != null == team.players.Contains(HumanPlayerController.localPlayer))
                ShipSelectionManager.instance.RemoveSelectableFromSelectionSets(entityId);
        }

        public ISelectable GetSelectionHandler() => shipSelectionHandler;
    }
}