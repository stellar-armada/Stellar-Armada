using Mirror;
using UnityEngine;
namespace SpaceCommander.Ships.Test
{
   public class ShipTest : MonoBehaviour
   {
      void Awake()
      {
         NetworkManager.singleton.StartServer();
      }
   }
}
