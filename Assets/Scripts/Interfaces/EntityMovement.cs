using SpaceCommander.Ships;
using UnityEngine;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace SpaceCommander
{
    public class EntityMovement: MonoBehaviour
    {
        bool controlEnabled = false;
        [HideInInspector] public Transform currentTarget;
        
        [SerializeField] NetworkEntity entity;
        
        NetworkEntity tempShip;
        private Transform transformToMoveTo; // for pursuit

        public SteerForPursuit steerForPursuit;
        public SteerForPoint steerForPoint;
        public AutonomousVehicle autonomousVehicle;

        void Awake()
        {
            entity = GetComponent<NetworkEntity>();
        }
        
        public void MoveToPointInSpace(Vector3 pos)
        {
            if (!controlEnabled || !entity.isServer) return;
            GoToPoint(pos);
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
        }
        
        void GoToPoint(Vector3 point)
        {
            steerForPoint.enabled = true;
            steerForPoint.TargetPoint = point;
            steerForPursuit.enabled = false;
        }

        public void EnableMovement()
        {
            if (!entity.isServer) return;
            controlEnabled = false;
        }

        public void DisableMovement()
        {
            if (!entity.isServer) return;
            controlEnabled = false;
        }
    }
}