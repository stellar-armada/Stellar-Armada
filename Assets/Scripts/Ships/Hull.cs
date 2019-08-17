using Mirror;
using SpaceCommander.Selection;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class Hull : NetworkBehaviour, IDamageable, ICollidable
    {
        [SerializeField] private Ship ship;

        public float maxHull;
        
        public delegate void HullChangeEvent(float shieldVal);

        public HullChangeEvent HullChanged;

        [SerializeField] SelectionHandler selectionHandler;

        [SyncVar] public float currentHull;

        void Awake()
        {
            currentHull = maxHull;
        }

        [Command]
        public void CmdRepairHull(float amount)
        {
            currentHull = Mathf.Min(currentHull + amount, maxHull);
        }

        public NetworkEntity GetOwningEntity()
        {
            return ship;
        }


        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void TakeDamage(float damage, Vector3 point, Damager damager)
        {
            if (!ship.IsAlive())
            {
                return;
            }

            currentHull -= damage;
            if (currentHull <= 0 && ship.IsAlive())
            {
                if(isServer)
                    ship.CmdDie();
            }

            HullChanged.Invoke(currentHull);
        }

        public IDamageable GetDamageable()
        {
            return this;
        }

        public ISelectable GetSelectable()
        {
            return selectionHandler;
        }
    }
}