using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class ShipMovement : NetworkBehaviour
    {
        [SyncVar] public bool controlEnabled = false;

        [HideInInspector] public Transform currentTarget;
        private Ship _ship;
        
        Ship _tempShip;
        private Transform transformToMoveTo;

        void Awake()
        {
            _ship = GetComponent<Ship>();
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
            _tempShip = ShipManager.GetShipByNetId(shipID);
            Transform shipToMoveTo = _tempShip.GetComponent<Transform>();
            currentTarget = shipToMoveTo;
            GoToPoint(shipToMoveTo.position);
        }
        
        [Command]
        public void CmdPursueShip(uint shipID)
        {
            if (!controlEnabled) return;
            _tempShip = ShipManager.GetShipByNetId(shipID);
            Transform shipToPursue = _tempShip.GetComponent<Transform>();
            _ship.steerForPoint.enabled = false;
            _ship.steerForPursuit.Quarry = _tempShip.autonomousVehicle;
            _ship.steerForPursuit.enabled = true;
            currentTarget = shipToPursue;
        }

        [Command]
        public void CmdStopMovement()
        {
            _ship.steerForPoint.TargetPoint = transform.position;
            _ship.steerForPoint.enabled = true;
            _ship.steerForPursuit.Quarry = null;
            _ship.steerForPursuit.enabled = false;
        }
        
        void GoToPoint(Vector3 point)
        {
            _ship.steerForPoint.enabled = true;
            _ship.steerForPoint.TargetPoint = point;
            _ship.steerForPursuit.enabled = false;
        }
    }
}