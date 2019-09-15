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

        [Command]
        public void CmdPursue(Transform target, bool isFriendly)
        {
            Debug.Log("<color=green>PURSUIT</green> Pursing entity!");
            NetworkEntity e = target.GetComponent<NetworkEntity>();
            PursueEntity(e.GetEntityId());
        }

        [Command]
        public void CmdMoveToPoint(Vector3 pos, Quaternion rot)
        {
            if (!controlEnabled)
            {
                Debug.LogError("Can't move, control not enabled");
            }

            entity.weaponSystemController.ClearTargets();
            GoToPoint(pos);
            OnMoveToPoint?.Invoke(pos, rot);
        }

        public void MoveToEntity(uint shipID)
        {
            if (!controlEnabled || !entity.isServer) return;
            tempShip = EntityManager.GetEntityById(shipID);
            Transform shipToMoveTo = tempShip.GetComponent<Transform>();
            GoToPoint(shipToMoveTo.position);
        }
        
        public void PursueEntity(uint entityId)
        {
            if (!controlEnabled || !entity.isServer) return;
            tempShip = EntityManager.GetEntityById(entityId);
            Transform shipToPursue = tempShip.GetComponent<Transform>();
            steerForPoint.enabled = false;
            steerForPursuit.Quarry = tempShip.GetComponent<EntityMovement>().autonomousVehicle;
            steerForPursuit.enabled = true;
        }
        
        public void StopMovement()
        {
            if (!entity.isServer) return;
            steerForPoint.TargetPoint = transform.position;
            steerForPoint.enabled = true;
            steerForPursuit.Quarry = null;
            steerForPursuit.enabled = false;
            OnArrival?.Invoke();
        }
        
        void GoToPoint(Vector3 point)
        {
            steerForPoint.enabled = true;
            steerForPoint.TargetPoint = point;
            steerForPursuit.enabled = false;
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