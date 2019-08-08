using Mirror;
using SpaceCommander.Game;
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
            CreateShip(ShipType.Bassilisk);
        }
        
        public void CreateBattlecruiser()
        {
            CreateShip(ShipType.Battlecruiser);
        }
        
        public void CreateDreadnaught()
        {
            CreateShip(ShipType.Dreadnaught);
        }
        
        public void CreateGuardian()
        {
            CreateShip(ShipType.Guardian);
        }
        
        public void CreateMarauder()
        {
            CreateShip(ShipType.Marauder);
        }
        
        public void CreatePhalanx()
        {
            CreateShip(ShipType.Phalanx);
        }
        
        public void CreateShip(ShipType shipShipType)
        {

            IPlayer player = PlayerManager.GetPlayers()[0];
            ShipFactory.instance.CmdCreateShipForPlayer(player.GetId(), shipShipType, Vector3.zero, Quaternion.identity);
        }
        
    }

}