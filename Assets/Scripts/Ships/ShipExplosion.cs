using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class ShipExplosion : NetworkBehaviour, IExplodable
    {
        [SerializeField] GameObject explosion;
        private float hideShip = 1.5f;
        private float deactivateShip = 6.5f;
        private Ship _ship;
        
        void Awake()
        {
            _ship = GetComponent<Ship>();
            _ship.ShipDestroyed.AddListener(Explode);
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
            _ship.visualModel.GetComponent<Renderer>().enabled = false;
            _ship.shipCollider.enabled = false;
        }

        void DeactivateShip()
        {
            gameObject.SetActive(false);
        }
    }
}