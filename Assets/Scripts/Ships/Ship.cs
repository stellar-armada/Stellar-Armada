using System;
using UnityEngine;
using Mirror;
using StellarArmada.Levels;
using StellarArmada.Player;
using StellarArmada.Teams;
using StellarArmada.UI;
using UnityEngine.Serialization;

// In the current iteration, ships are organized into three formation roles
// Ships will be stacked in a grid adjacent to other ships in their line
public enum FormationPosition
{
    Frontline = 0,
    Midline = 1,
    Backline = 2
}

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Ships are a special type of network entity that can be controlled by a commander
    // Ships are owned by teams, but may have a captain
    public class Ship : NetworkBehaviour
    {
       

        // An enum identifying what type of ship this is
        public ShipType type;

        // handy reference to which group this ship is in
        public int group = -1;

        // Where in the formation should this ship be? Frontline, backline, etc.
        public FormationPosition formationPosition;

        [Header("Ship Subsystems")] public ShipBridge bridge; //Instantiated and set when the captain choose this ship

        public ShipWarp
            shipWarp; // Manages the visual ship warp, as well as enabling the ship for command when warped in

        [FormerlySerializedAs("miniMapStatusBar")] public ShipStatusBar shipStatusBar; // Insignia, health/shields and captain nameplate in the minimap
        public ShipSelectionHandler shipSelectionHandler; // Handles all logic related to local player selection

        public Action OnCaptainUpdated = delegate { }; // Delegate called when a captain is set for the ship

        [Server] // Server-side logic
        public void ServerSetCaptain(uint playerId)
        {
            captain = PlayerManager.GetPlayerById(playerId);
            RpcSetCaptain(playerId);
        }

        [ClientRpc] // Client-side logic
        public void RpcSetCaptain(uint playerId)
        {
            Debug.Log("Rpc Set Captain called");
            // Prevent double calls on host
            captain = PlayerManager.GetPlayerById(playerId);
            Debug.Log("captain: " + captain);

            // Local player logic
            if (captain == PlayerController.localPlayer)
            {
                Debug.Log("captain == PlayerController.localPlayer");

                bridge.ActivateBridgeForLocalPlayer();
                ShipSelectionManager.instance.InitializeSelectionSets();
                GroupUIManager.instance.UpdateGroupManager();
                PlayerCamera.instance.ShowMatchView(); // Switch camera layers to minimap and ships for local player
                OnCaptainUpdated?.Invoke();
            }
        }

        public PlayerController GetCaptain() => captain;

        // Change battlegroup for ship for local player
        void SetGroup(int newGroupId)
        {
            team.ChangeEntityGroup(this, newGroupId);
        }

        [Server] // Server logic
        public void ServerSetTeam(uint newTeamId)
        {
            if(isServerOnly) // Player needs to be added to server, who won't receive RPC callback
                SetTeam(newTeamId);
            RpcSetTeam(newTeamId);
        }

        [ClientRpc] // Client logic
        public void RpcSetTeam(uint newTeamId)
        {
                SetTeam(newTeamId);
        }

        void SetTeam(uint newTeamId) // Called on both server and client
        {
            if (team != null && team.entities.Contains(this) && team.teamId != newTeamId) team.RemoveEntity(this);
            team = TeamManager.instance.GetTeamByID(newTeamId);
            team.AddEntity(this);
            shipStatusBar.SetInsignia(team.insignia);
            
            OnTeamChange?.Invoke();
        }

        // Call this to kill a ship -- should only ever be called server side, since nobody has local authority over entities
        [Command]
        public void CmdDie()
        {
            Die();
            RpcDie();
        }

        [ClientRpc]
        public void RpcDie()
        {
            Die();
        }

        void Die()
        {
            // TO DO -- move these all to delegate subscriptions instead of direct calls
            isAlive = false;
            shipStatusBar.FadeOutStatusBar();
            shipExplosion.Explode();
            weaponSystemController.weaponSystemsEnabled = false;
            weaponSystemController.HideWeaponSystems();
            shield.currentShield = 0;
            shield.gameObject.SetActive(false);
            if (PlayerController.localPlayer != null == team.players.Contains(PlayerController.localPlayer))
                ShipSelectionManager.instance.RemoveSelectableFromSelectionSets(entityId);
        }

        public ISelectable GetSelectionHandler() => shipSelectionHandler;
        
         // For this implementation, entities can be captained by a single player
        // Captains are placed on the bridge, which is instantiated for all captained ships
        public PlayerController captain;
        
        // All network entities start alive and can be killed
        // When isAlive is changed on the network, call the HandleDeath() callback
        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;
        
        [Header("Entity Subsystems")]
        public ShipHull hull; // Handles damage to the ship
        public ShipShield shield; // Reference to the shield, which blocks hull damage
        public ShipWeaponSystemController weaponSystemController; // Manages the various weapon systems on the ship, especially enabling/disabling or targeting/focus fire
        
        // All entities are assigned to a team in the factory
        // In an FFA, each player would be assigned to their own team
        protected Team team;

        // Unique identifier for each entity. Set by any factory inheriting from the EntityFactory baseclass
        [SyncVar] protected uint entityId;
        
        [Header("Entity Subsystems")]
        // All entity movement is handled here. If !movement.canMove, then movement is ignored
        public ShipMovement movement;
        
        // A sequence of particle effects and timers played when the entity dies
        [FormerlySerializedAs("entityExplosion")] public ShipExplosion shipExplosion;
        
        public Renderer visualModel; // Visual model of the ship -- moved when warping in, hidden when killed

        // A representation of the entity on the minimap for the local human player
        public MiniMapEntity miniMapEntity;

        public delegate void NetworkEntityEvent();
        
        // Delegate called when entity dies
        public NetworkEntityEvent OnEntityDead;
        public NetworkEntityEvent OnTeamChange;

        void HandleDeath(bool alive)
        {
            if (!alive)
            {
                OnEntityDead?.Invoke();
            }
        }

        public Team GetTeam() => team;

        public uint GetEntityId() => entityId;

        public void SetEntityId(uint id) => entityId = id;

        public bool IsAlive()
        {
            if(!isAlive) Debug.LogError("Entity is alive: " + isAlive);
            return isAlive;
        }
        
        public GameObject GetGameObject() => gameObject;
        
        void Start()
        {
            transform.SetParent(LevelRoot.instance.transform, true);
            ShipManager.instance.RegisterEntity(this);
        }

        void OnDestroy() => ShipManager.instance.UnregisterEntity(this);

        // Enemy-ness is currently checked by whether or not both entities are on the same team
        // In the future we may wish to add alliance logic here
        public bool IsEnemy(Ship otherShip)
        {
            if (otherShip.GetTeam().teamId != GetTeam().teamId) return true;
            return false;
        }
}
}