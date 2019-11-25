using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipExplosion : NetworkBehaviour, IExplodable
    {
        [SerializeField] Renderer[] renderersToHideOnExplosion;
        [SerializeField] Collider[] collidersToDisableOnExplosion;
        [SerializeField] GameObject[] explosions;
        private float hideShip = 1.5f;
        private float deactivateShip = 6.5f;
        private Ship _ship;
        
        void Awake()
        {
            _ship = GetComponent<Ship>();
            _ship.OnEntityDead += Explode;
        }
        
        public void Explode()
        {
            foreach(var explosion in explosions)
                explosion.SetActive(true);
            Invoke(nameof(HideShip), hideShip);
            Invoke(nameof(DeactivateShip), deactivateShip);
        }
        
        void HideShip()
        {
            foreach (Renderer ren in renderersToHideOnExplosion)
            {
                ren.enabled = false;
            }

            foreach (Collider col in collidersToDisableOnExplosion)
            {
                col.enabled = false;
            }
        }

        void DeactivateShip()
        {
            _ship.shield.isEnaled = false;
            _ship.shield.currentShield = 0;

            _ship.miniMapEntity.Deactivate();
        }
    }
}