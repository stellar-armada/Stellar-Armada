using System.Collections.Generic;
using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Players;
using SpaceCommander.Scenarios;
using SpaceCommander.Selection;
using SpaceCommander.Teams;
using SpaceCommander.UI;
using UnityEngine.Events;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class Ship : NetworkBehaviour, IEntity
    {
        public PlayerController playerController; // Ships are only given to players in FFA or DOTA modes, not implemented yet

        public ShipType type;
        [Header("Ship Subsystems")]
        public ShipMovement shipMovement;
        public ShipHull shipHull;
        public ShipShield shipShield;
        public ShipWeaponSystemController weaponSystemController;
        public ShipExplosion shipExplosion;
        public ShipWarp shipWarp;
        public StatusBar statusBar;
        public ShipSelectionHandler selectionHandler;
        
        [Header("UnitySteer Steering Systems")]
        public Radar radar;
        public AutonomousVehicle autonomousVehicle;
        public SteerForPursuit steerForPursuit;
        public SteerForPoint steerForPoint;
        
        [Header("Ship Components")]
        public Collider shipCollider;
        public Renderer visualModel;
        
        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;
        
        private Team team; // Should be set by ship factory. Referenced in enemy checks.

        [SyncVar] [SerializeField] uint entityId;
        
        public UnityEvent ShipDestroyed;
        
        void Awake()
        {
            transform.parent = MapParent.instance.transform;
            transform.localScale = Vector3.one;
        }

        void HandleDeath(bool alive)
        {
            Debug.Log("Handling death");
            if (!alive)
            {
                ShipDestroyed.Invoke();
            }
        }

        [Command]
        public void CmdSetPlayer(uint playerId)
        {
            playerController = PlayerManager.GetPlayerById(playerId);
        }

        public PlayerController GetPlayer()
        {
            return playerController;
        }

        void SetGroup(int newGroupId)
        {
            team.ChangeEntityGroup(this, newGroupId);
        }
        
        [Command]
        public void CmdSetTeam(uint newTeamId)
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

        public Team GetTeam()
        {
            return team;
        }

        public uint GetEntityId()
        {
            return entityId;
        }

        public void SetEntityId(uint id)
        {
            entityId = id;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public bool IsEnemy(IEntity otherEntity)
        {
            if (otherEntity.GetTeam().teamId != GetTeam().teamId) return true; // For now, players will all shoot each other FFA if they are controller "player ships"
            return false;
        }
        
        [Command]
        public void CmdActivate()
        {
            isAlive = true;
            statusBar.ShowStatusBar(); 
            weaponSystemController.weaponSystemsEnabled = true;
        }
        
        [Command]
        public void CmdDie()
        {
            isAlive = false;
            statusBar.FadeOutStatusBar();
            shipExplosion.Explode();
            weaponSystemController.weaponSystemsEnabled = false;
            weaponSystemController.HideWeaponSystems();
            shipShield.currentShield = 0;
            shipShield.gameObject.SetActive(false);

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
        
        public GameObject GetGameObject() => gameObject;

        public ISelectable GetSelectionHandler() => selectionHandler;

        void Start() => ShipManager.instance.RegisterShip(this);

        void OnDestroy() => ShipManager.instance.UnregisterShip(this);
    }
}