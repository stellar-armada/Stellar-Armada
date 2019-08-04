using Mirror;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipExplosion : NetworkBehaviour, IExplodable
    {
        [SerializeField] GameObject explosion;
        private float hideShip = 1.5f;
        private float deactivateShip = 6.5f;
        private Ship ship;
        
        void Awake()
        {
            ship = GetComponent<Ship>();
            ship.ShipDestroyed.AddListener(Explode);
        }
        
        public void Explode()
        {
            Debug.Log("Exploding");
            explosion.SetActive(true);
            Invoke(nameof(HideShip), hideShip);
            Invoke(nameof(DeactivateShip), deactivateShip);
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