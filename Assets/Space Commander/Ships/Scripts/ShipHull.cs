using Mirror;
using SpaceCommander.Selection;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander.Ships
{
    public class ShipHull : NetworkBehaviour, IDamageable, ICollidable
    {
        [SerializeField] private Ship ship;

        public float maxHull;

        public UnityEvent HullChanged;

        [SerializeField] ShipSelectionHandler selectionHandler;
        
        [SyncVar (hook=nameof(HandleHullChange))] public float currentHull;

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
            if (!ship.IsAlive()) return;
                currentHull -= damage;
            if (currentHull <= 0 && ship.IsAlive())
            {
                ship.CmdDie();
            }
        }
        
        void HandleHullChange(float h)
        {
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

