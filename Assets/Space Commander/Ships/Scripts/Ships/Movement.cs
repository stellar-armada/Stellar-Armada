using Mirror;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class Movement : NetworkBehaviour
    {
        [SyncVar] public bool controlEnabled = false;

        [HideInInspector] public Transform currentTarget;
        private Ship ship;
        
        Ship tempShip;
        private Transform transformToMoveTo;

        void Awake()
        {
            ship = GetComponent<Ship>();
        }
        [Command]
        public void CmdMoveToPointInSpace(Vector3 pos)
        {
            if (!controlEnabled) return;
            GoToPoint(pos);
        }

        [Command]
        public void CmdMoveToShip(uint shipID)
        {
            if (!controlEnabled) return;
            tempShip = ShipManager.GetShipByNetId(shipID);
            Transform shipToMoveTo = tempShip.GetComponent<Transform>();
            currentTarget = shipToMoveTo;
            GoToPoint(shipToMoveTo.position);
            Debug.Log("Moving to ship");
        }
        
        [Command]
        public void CmdPursueShip(uint shipID)
        {
            if (!controlEnabled) return;
            tempShip = ShipManager.GetShipByNetId(shipID);
            Transform shipToPursue = tempShip.GetComponent<Transform>();
            ship.steerForPoint.enabled = false;
            ship.steerForPursuit.Quarry = tempShip.autonomousVehicle;
            ship.steerForPursuit.enabled = true;
            currentTarget = shipToPursue;
            Debug.Log("Pursuing ship");
        }

        [Command]
        public void CmdStopMovement()
        {
            ship.steerForPoint.TargetPoint = transform.position;
            ship.steerForPoint.enabled = true;
            ship.steerForPursuit.Quarry = null;
            ship.steerForPursuit.enabled = false;
        }
        
        void GoToPoint(Vector3 point)
        {
            ship.steerForPoint.enabled = true;
            ship.steerForPoint.TargetPoint = point;
            ship.steerForPursuit.enabled = false;
        }
    }
}