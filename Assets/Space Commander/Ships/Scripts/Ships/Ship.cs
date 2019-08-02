using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using SpaceCommander.Game;
using UnitySteer.Behaviors;

namespace SpaceCommander.Ships
{

    public class Ship : NetworkBehaviour, IDamager, IDamageable
    {
        public readonly ShipType shipType;
        public IPlayer player;

        [Header("Ship Subsystems")]
        public Movement movement;
        public Health health;
        public Explosion explosion;
        public Warp warp;
        public UI ui;
        
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
        public void CmdSetShipOwner(uint playerID)
        {
            player = PlayerManager.GetPlayerByNetID(playerID);
        }
        
        public uint GetShipId()
        {
            return netId;
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

        public IPlayer GetPlayer()
        {
            throw new System.NotImplementedException();
        }

        void IDamageable.SetPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        void IDamager.SetPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }
    }
}