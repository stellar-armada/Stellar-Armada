using System.Collections;
using System.Collections.Generic;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Ships;
using UnityEngine;

// Test the ship factory, which creates new ship objects for players
// 

namespace SpaceCommander.Ships.Tests
{
    public class ShipFactoryTest : MonoBehaviour
    {
        [SerializeField] GameObject sharedNetworkManagers;

        void Start()
        {
            NetworkManager.singleton.StartHost();
            GameObject sharedNetworkManagerObject = Instantiate(sharedNetworkManagers);
            NetworkServer.Spawn(sharedNetworkManagerObject);
        }
        
        public void CreateBassilisk()
        {
            CreateShip(Type.Bassilisk);
        }
        
        public void CreateBattlecruiser()
        {
            CreateShip(Type.Battlecruiser);
        }
        
        public void CreateDreadnaught()
        {
            CreateShip(Type.Dreadnaught);
        }
        
        public void CreateGuardian()
        {
            CreateShip(Type.Guardian);
        }
        
        public void CreateMarauder()
        {
            CreateShip(Type.Marauder);
        }
        
        public void CreatePhalanx()
        {
            CreateShip(Type.Phalanx);
        }
        
        public void CreateShip(Type shipType)
        {

            IPlayer player = PlayerManager.GetPlayers()[0];
            ShipFactory.instance.CmdCreateShip(player.GetId(), shipType, Vector3.zero, Quaternion.identity);
        }
        
    }

}