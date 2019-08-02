using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class Explosion : NetworkBehaviour
    {
        [SerializeField] GameObject explosion;
        
        private Ship ship;
        
        void Awake()
        {
            ship = GetComponent<Ship>();
        }
        
        [Command]
        public void CmdExplode()
        {
            ship.movement.controlEnabled = false;

            RpcExplode();
        }

        [ClientRpc]
        public void RpcExplode()
        {
            explosion.SetActive(true);
            Invoke(nameof(HideShip), 1.5f);
            Invoke(nameof(DeactivateShip), 6.5f);

        }
        
        void HideShip()
        {
            ship.visualModel.GetComponent<Renderer>().enabled = false;
            ship.shipCollider.enabled = false;
        }

        void DeactivateShip()
        {
            gameObject.SetActive(false);
        }
    }
}