using Mirror;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Ships.Test
{
   public class ShipTest : MonoBehaviour
   {
      void Awake()
      {
         NetworkManager.singleton.StartServer();
      }
   }
}
