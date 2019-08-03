using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Weapons;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipWeaponSystemController : MonoBehaviour, IWeaponSystemController
    {
        public IPlayerEntity entity;

        List<IWeaponSystem> weaponSystems = new List<IWeaponSystem>();

        public bool weaponSystemsEnabled = false;

        void Awake()
        {
            entity = GetComponent<IPlayerEntity>();
        }
        public IPlayerEntity GetEntity()
        {
            return entity;
        }

        public void SetOwner(IPlayerEntity newOwner)
        {
            entity = newOwner;
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
            return weaponSystemsEnabled;
        }
    }
}