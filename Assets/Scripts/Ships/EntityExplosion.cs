using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander
{
    public class EntityExplosion : NetworkBehaviour, IExplodable
    {
        [SerializeField] Renderer[] renderersToHideOnExplosion;
        [SerializeField] Collider[] collidersToDisableOnExplosion;
        [SerializeField] GameObject explosion;
        private float hideShip = 1.5f;
        private float deactivateShip = 6.5f;
        private NetworkEntity entity;
        
        void Awake()
        {
            entity = GetComponent<NetworkEntity>();
            entity.OnEntityDead += Explode;
        }
        
        public void Explode()
        {
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
            gameObject.SetActive(false);
        }
    }
}