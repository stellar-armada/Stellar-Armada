using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using UnitySteer.Behaviors;

namespace SpaceCommander.Ships
{

    public class Ship : NetworkBehaviour, IPlayerOwnedEntity
    {
        public readonly ShipType shipType;
        public IPlayer player;

        [Header("Ship Subsystems")]
        public ShipMovement shipMovement;
        public ShipHealth shipHealth;
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
        
        [Command]
        public void CmdSetPlayer(uint playerID)
        {
            player = PlayerManager.GetPlayerByNetID(playerID);
        }
        
        public uint GetId()
        {
            return netId;
        }

        public IPlayer GetPlayer()
        {
            return player;
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