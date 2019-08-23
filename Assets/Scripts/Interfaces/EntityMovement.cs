using Mirror;
using StellarArmada.Ships;
using UnityEngine;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace StellarArmada
{
    public class EntityMovement: NetworkBehaviour
    {
        [SyncVar] public bool controlEnabled = false;
        [HideInInspector] public Transform currentTarget;
        
        [SerializeField] NetworkEntity entity;
        
        NetworkEntity tempShip;
        private Transform transformToMoveTo; // for pursuit

        public SteerForPursuit steerForPursuit;
        public SteerForPoint steerForPoint;
        public AutonomousVehicle autonomousVehicle;

        public delegate void MoveToPointEvent(Vector3 pos, Quaternion orientation);

        public delegate void MovementEvent();

        public MoveToPointEvent OnMoveToPoint;
        public MovementEvent OnStop;

        void Awake()
        {
            entity = GetComponent<NetworkEntity>();
        }
        
        [Command]
        public void CmdMoveToPoint(Vector3 pos, Quaternion rot)
        {
            if (!controlEnabled)
            {
                
                Debug.LogError("Can't move, control not enabled");
            }
            GoToPoint(pos);
            OnMoveToPoint?.Invoke(pos, rot);
        }

        public void MoveToEntity(uint shipID)
        {
            if (!controlEnabled || !entity.isServer) return;
            tempShip = EntityManager.GetEntityById(shipID);
            Transform shipToMoveTo = tempShip.GetComponent<Transform>();
            currentTarget = shipToMoveTo;
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
            currentTarget = shipToPursue;
        }
        
        public void StopMovement()
        {
            if (!entity.isServer) return;
            steerForPoint.TargetPoint = transform.position;
            steerForPoint.enabled = true;
            steerForPursuit.Quarry = null;
            steerForPursuit.enabled = false;
            OnStop?.Invoke();
        }
        
        void GoToPoint(Vector3 point)
        {
            steerForPoint.enabled = true;
            steerForPoint.TargetPoint = point;
            steerForPursuit.enabled = false;
        }

        [Command]
        public void CmdEnableMovement()
        {
            controlEnabled = false;
        }

        [Command]
        public void CmdDisableMovement()
        {
            OnStop?.Invoke();
            controlEnabled = false;
            
        }
    }
}