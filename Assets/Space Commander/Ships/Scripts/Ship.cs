using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnitySteer.Behaviors;

namespace SpaceCommander.Ships
{
    public class Ship : NetworkBehaviour, IPlayerEntity, ITeamEntity
    {
        public IPlayer player;

        [Header("Ship Subsystems")]
        public ShipMovement shipMovement;
        [FormerlySerializedAs("shipHealth")] public ShipHull shipHull;
        public ShipShield shipShield;
        public ShipExplosion shipExplosion;
        public ShipWarp shipWarp;
        public ShipUI shipUi;
        
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
        
        public UnityEvent ShipDestroyed;


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
        
        [Command]
        public void CmdSetGroup(int newGroupId)
        {
            SetGroup(newGroupId);
            RpcSetGroup(newGroupId);
        }

        [ClientRpc]
        public void RpcSetGroup(int newGroupId)
        {
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
            SetTeam(newTeamId);
        }

        void SetTeam(uint newTeamId)
        {
            if (team != null && team.entities.Contains(this) && team.teamId != newTeamId) team.RemoveEntity(this);
            team = TeamManager.instance.GetTeamByID(newTeamId);
            team.AddEntity(this);
        }

        public Team GetTeam()
        {
            return team;
        }

        public uint GetEntityId()
        {
            return netId;
        }

        public IPlayer GetPlayer()
        {
            return player;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public void CmdDie()
        {
            isAlive = false;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        void Awake()
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