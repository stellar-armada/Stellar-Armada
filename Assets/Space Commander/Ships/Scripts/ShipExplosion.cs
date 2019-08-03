using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

namespace SpaceCommander.Ships
{
    public class ShipExplosion : NetworkBehaviour, IExplodable
    {
        [SerializeField] GameObject explosion;
        
        private Ship ship;
        
        void Awake()
        {
            ship = GetComponent<Ship>();
            ship.shipHealth.ShipDestroyed.AddListener(Explode);
        }
        
        public void Explode()
        {
            Debug.Log("Exploding");
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