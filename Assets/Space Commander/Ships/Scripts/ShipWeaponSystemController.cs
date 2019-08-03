using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Weapons;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipWeaponSystemController : MonoBehaviour, IWeaponSystemController
    {
        public IPlayerOwnedEntity owner;

        List<IWeaponSystem> weaponSystems = new List<IWeaponSystem>();

        private bool weaponSystemsEnabled = false;

        void Awake()
        {
            owner = GetComponent<IPlayerOwnedEntity>();
        }
        public IPlayerOwnedEntity GetOwner()
        {
            return owner;
        }

        public void SetOwner(IPlayerOwnedEntity newOwner)
        {
            owner = newOwner;
        }

        public List<IWeaponSystem> GetWeaponSystems()
        {
            return weaponSystems;
        }

        public void EnableWeaponSystems()
        {
            weaponSystemsEnabled = true;
        }

        public void DisableWeaponSystems()
        {
            weaponSystemsEnabled = false;
            foreach (IWeaponSystem weaponSystem in weaponSystems)
            {
                weaponSystem.SetTarget(null);
            }
        }

        public void RegisterWeaponSystem(WeaponSystem weaponSystem)
        {
            weaponSystems.Add(weaponSystem);
        }

        public bool WeaponSystemsEnabled()
        {
            throw new System.NotImplementedException();
        }
    }
}