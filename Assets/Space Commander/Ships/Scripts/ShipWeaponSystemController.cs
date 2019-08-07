using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipWeaponSystemController : MonoBehaviour, IWeaponSystemController
    {

        [SerializeField] private Ship ship;
        
        List<IWeaponSystem> weaponSystems = new List<IWeaponSystem>();

        public bool weaponSystemsEnabled = false;

        public IPlayerEntity GetEntity()
        {
            return ship;
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