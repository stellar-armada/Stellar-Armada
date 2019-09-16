using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityMovement: NetworkBehaviour
    {
        // Handles movement for all network entities, including ships
        
        [SyncVar (hook=nameof(HandleControlChanged))] public bool controlEnabled = false;  // Control enabled -- can this entity be controlled by commanders on the team?
        
        public NetworkEntity entity; // Reference to the owning entity

        [SerializeField] private List<Steering> steeringBehaviors = new List<Steering>();
        
        [SerializeField] SteerForPursuit steerForPursuit;
        [SerializeField] SteerForPoint steerForPoint;
        [SerializeField] AutonomousVehicle autonomousVehicle;

        public delegate void MoveToPointEvent(Vector3 pos, Quaternion orientation);
        public delegate void MovementEvent();

        public MoveToPointEvent OnMoveToPoint; // Delegate called when ship starts to move
        public MovementEvent OnArrival; // Delegate called when ship arrives or stops at destination

        // Reusable variables
        NetworkEntity tempShip;
        Transform transformToMoveTo; // for pursuit

        void HandleControlChanged(bool newControlEnabledState)
        {
            foreach (Steering behavior in steeringBehaviors)
            {
                behavior.enabled = newControlEnabledState;
            }
        }
        
        void Awake()
        {
            entity = GetComponent<NetworkEntity>();
            steerForPoint.OnArrival += HandleArrival;
            HandleControlChanged(controlEnabled); // Disable steering behaviors by default
        }

        // Callback for SteerForPoint's OnArrival, so subscribers don't need to connect to steering components directly
        private void HandleArrival(Steering obj)
        {
            OnArrival?.Invoke();
        }

        [Server]
        public void ServerPursue(uint entityId)
        {
            if (!controlEnabled) return;
            if(isServerOnly) Pursue(EntityManager.GetEntityById(entityId));
            RpcPursue(entityId);
        }

        [ClientRpc]
        void RpcPursue(uint entityId)
        {
            Pursue(EntityManager.GetEntityById(entityId));
        }

        void Pursue(NetworkEntity entity)
        {
            entity.weaponSystemController.ClearTargets();
            steerForPoint.enabled = false;
            steerForPursuit.Quarry = entity.GetComponent<EntityMovement>().autonomousVehicle;
            steerForPursuit.enabled = true;
        }

        [Server]
        public void ServerMoveToPoint(Vector3 pos, Quaternion rot)
        {
            if (!controlEnabled)
            {
                Debug.Log("Can't move, control not enabled");
                return;
            }

            if (isServerOnly) MoveToPoint(pos, rot);
            
            RpcMoveToPoint(pos, rot);
        }

        [ClientRpc]
        void RpcMoveToPoint(Vector3 pos, Quaternion rot) => MoveToPoint(pos, rot);

        void MoveToPoint(Vector3 pos, Quaternion rot)
        {
            steerForPoint.enabled = true;
            steerForPoint.TargetPoint = pos;
            steerForPursuit.enabled = false;
            OnMoveToPoint?.Invoke(pos, rot); 
        }

        [Server]
        public void ServerStopMovement()
        {
            if(isServerOnly) StopMovement();
            RpcStopMovement();
        }

        [ClientRpc]
        void RpcStopMovement() => StopMovement();

        void StopMovement()
        {
            entity.weaponSystemController.ClearTargets();
            steerForPoint.TargetPoint = transform.position;
            steerForPoint.enabled = true;
            steerForPursuit.Quarry = null;
            steerForPursuit.enabled = false;
            OnArrival?.Invoke();
        }
        
        public void EnableMovement()
        {
            controlEnabled = true;
        }
        
        public void DisableMovement()
        {
            controlEnabled = false;
        }
    }
}