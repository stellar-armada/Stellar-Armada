using System.Collections.Generic;
using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Scenarios;
using SpaceCommander.Selection;
using SpaceCommander.Teams;
using UnityEngine.Events;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class Ship : NetworkBehaviour, ITeamEntity, IPlayerEntity
    {
        public IPlayer player; // Ships are only given to players in FFA or DOTA modes, not implemented yet

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
        public GameObject hologram;
        
        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;
        
        private Team team; // Should be set by ship factory. Referenced in enemy checks.

        [SyncVar] [SerializeField] uint entityId;
        
        public UnityEvent ShipDestroyed;

        [SerializeField] List<EntityType> entityTypes = new List<EntityType> {EntityType.TEAM}; // Team for now, add player if we want players to own their own ships later
        
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
            player = PlayerManager.GetPlayerById(playerId);
        }

        public IPlayer GetPlayer()
        {
            return player;
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

        public (List<EntityType>, IEntity) GetEntityAndTypes()
        {
            return (entityTypes, this);
        }
        
        public bool IsAlive()
        {
            return isAlive;
        }

        public bool IsEnemy(IEntity otherEntity)
        {
            var entityKey = otherEntity.GetEntityAndTypes();
                var entityTypes = entityKey.Item1;
                if (entityTypes.Contains(EntityType.TEAM))
                {
                    if (((ITeamEntity)otherEntity).GetTeam().teamId != GetTeam().teamId) return true; // For now, players will all shoot each other FFA if they are controller "player ships"
                }
                else
                {
                    Debug.Log("Cant check if enemy, doesn't appear to be type TEAM");
                }

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
        }
        

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public ISelectable GetSelectionHandler()
        {
            return selectionHandler;
        }

        void Start()
        {
            ShipManager.instance.RegisterShip(this);
        }

        void OnDestroy()
        {
            ShipManager.instance.UnregisterShip(this);
        }
        
        public void HideHologram()
        {
            hologram.GetComponent<Renderer>().enabled = false;
        }

        public void ShowHologram()
        {
            hologram.GetComponent<Renderer>().enabled = true;
        }
    }
}