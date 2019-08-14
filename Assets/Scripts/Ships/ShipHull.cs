using Mirror;
using SpaceCommander.Selection;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class ShipHull : NetworkBehaviour, IDamageable, ICollidable
    {
        [SerializeField] private Ship ship;

        public float maxHull;

        public UnityEvent HullChanged;

        [SerializeField] ShipSelectionHandler selectionHandler;

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

        public IEntity GetOwningEntity()
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
                Debug.Log("Can't take damage, ship is dead");
                return;
            }

            currentHull -= damage;
            if (currentHull <= 0 && ship.IsAlive())
            {
                ship.CmdDie();
                Debug.Log("Ship is dead");
            }

            HullChanged.Invoke();
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