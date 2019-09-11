using Mirror;
using StellarArmada.Levels;
using StellarArmada.Player;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    // Base class for entities that are shared over the network
    // Ships, stations, fighters, missiles, etc. inherit from this class 
    public abstract class NetworkEntity : NetworkBehaviour
    {
        // All network entities start alive and can be killed
        // When isAlive is changed on the network, call the HandleDeath() callback
        [SyncVar(hook = nameof(HandleDeath))] public bool isAlive = true;
        
        [Header("Entity Subsystems")]
        public EntityHull hull; // Handles damage to the ship
        public EntityShield shield; // Reference to the shield, which blocks hull damage
        public EntityWeaponSystemController weaponSystemController; // Manages the various weapon systems on the ship, especially enabling/disabling or targeting/focus fire
        
        // All entities are assigned to a team in the factory
        // In an FFA, each player would be assigned to their own team
        protected Team team;

        // Unique identifier for each entity. Set by any factory inheriting from the EntityFactory baseclass
        [SyncVar] protected uint entityId;
        
        [Header("Entity Subsystems")]
        // All entity movement is handled here. If !movement.canMove, then movement is ignored
        public EntityMovement movement;
        
        // A sequence of particle effects and timers played when the entity dies
        public EntityExplosion entityExplosion;
        
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

        public bool IsAlive() => isAlive;
        
        public GameObject GetGameObject() => gameObject;
        
        void Start()
        {
            transform.SetParent(LevelRoot.instance.transform, true);
            EntityManager.instance.RegisterEntity(this);
        }

        void OnDestroy() => EntityManager.instance.UnregisterEntity(this);

        // Enemy-ness is currently checked by whether or not both entities are on the same team
        // In the future we may wish to add alliance logic here
        public bool IsEnemy(NetworkEntity otherNetworkEntity)
        {
            if (otherNetworkEntity.GetTeam().teamId != GetTeam().teamId) return true;
            return false;
        }

        // 
        [Command]
        public virtual void CmdDie()
        {
            if (isServerOnly)
            {
                foreach(PlayerController pc in GetComponentsInChildren<PlayerController>())
                {
                    pc.Die();
                    isAlive = false;
                }
            }
            RpcDie();
        }

        [ClientRpc]
        public virtual void RpcDie()
        {
            isAlive = false;
        }
        
        public abstract void CmdSetTeam(uint teamId);
    }
}