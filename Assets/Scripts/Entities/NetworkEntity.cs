using Mirror;
using SpaceCommander.Scenarios;
using SpaceCommander.Selection;
using SpaceCommander.Ships;
using SpaceCommander.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander
{
    public abstract class NetworkEntity : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;
        
        protected Team team; // Should be set by ship factory. Referenced in enemy checks.

        [SyncVar] protected uint entityId;

        [Header("Entity Subsystems")]
        public EntityMovement movement;
        public EntityExplosion entityExplosion;

        public SelectionHandler selectionHandler;

        public delegate void NetworkEntityEvent();
        
        public NetworkEntityEvent OnEntityDead;
        
        void HandleDeath(bool alive)
        {
            if (!alive)
            {
                OnEntityDead.Invoke();
            }
        }
        
        void Awake()
        {
            transform.parent = MapParent.instance.transform;
            transform.localScale = Vector3.one;
        }
        
        public Team GetTeam() => team;

        public uint GetEntityId() => entityId;

        public void SetEntityId(uint id) => entityId = id;

        public bool IsAlive() => isAlive;
        
        public GameObject GetGameObject() => gameObject;
        
        void Start() => EntityManager.instance.RegisterEntity(this);

        void OnDestroy() => EntityManager.instance.UnregisterEntity(this);
        
        public ISelectable GetSelectionHandler() => selectionHandler;

        public bool IsEnemy(NetworkEntity otherNetworkEntity)
        {
            if (otherNetworkEntity.GetTeam().teamId != GetTeam().teamId) return true; // For now, players will all shoot each other FFA if they are controller "player ships"
            return false;
        }

        public virtual void Die()
        {
            isAlive = false;
        }
        public abstract void CmdSetTeam(uint teamId);
    }
}