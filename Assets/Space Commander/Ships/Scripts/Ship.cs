using System.Collections.Generic;
using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnitySteer.Behaviors;

namespace SpaceCommander.Ships
{
    public class Ship : NetworkBehaviour, ITeamEntity, IPlayerEntity
    {
        public IPlayer player; // Ships are only given to players in FFA or DOTA modes

        [Header("Ship Subsystems")]
        public ShipMovement shipMovement;
        public ShipHull shipHull;
        public ShipShield shipShield;
        public ShipWeaponSystemController weaponSystemController;
        public ShipExplosion shipExplosion;
        public ShipWarp shipWarp;
        public StatusBar statusBar;
        
        [Header("UnitySteer Steering Systems")]
        public Radar radar;
        public AutonomousVehicle autonomousVehicle;
        public SteerForPursuit steerForPursuit;
        public SteerForPoint steerForPoint;
        
        [Header("Ship Components")]
        public Collider shipCollider;
        public GameObject visualModel;
        public GameObject hologram;

        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;

        public delegate void TeamChangeDelegate(uint newTeam);
        
        private Team team;

        [SyncVar] [SerializeField] uint entityId;
        
        public UnityEvent ShipDestroyed;

        [SerializeField] List<EntityType> entityTypes = new List<EntityType> {EntityType.TEAM};
        
        void Awake()
        {
            transform.parent = SceneRoot.instance.transform;
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
            player = PlayerManager.GetPlayerByNetId(playerId);
        }

        public IPlayer GetPlayer()
        {
            return player;
        }

        [Command]
        public void CmdSetGroup(int newGroupId)
        {
            SetGroup(newGroupId);
            RpcSetGroup(newGroupId);
        }

        [ClientRpc]
        public void RpcSetGroup(int newGroupId)
        {
            if(!isServer) // Prevent double calls if we're testing in host mode
                SetGroup(newGroupId);
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

        public void CmdDie()
        {
            isAlive = false;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
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