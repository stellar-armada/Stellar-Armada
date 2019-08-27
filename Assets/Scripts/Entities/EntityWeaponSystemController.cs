using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityWeaponSystemController : MonoBehaviour, IWeaponSystemController
    {

        [SerializeField] private NetworkEntity entity;
        
        List<WeaponSystem> weaponSystems = new List<WeaponSystem>();

        public bool weaponSystemsEnabled = false;

        public NetworkEntity GetEntity()
        {
            return entity;
        }

        public List<WeaponSystem> GetWeaponSystems()
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
            foreach (WeaponSystem weaponSystem in weaponSystems)
            {
                weaponSystem.ClearTarget();
            }
        }

        public void HideWeaponSystems()
        {
            foreach (WeaponSystem weaponSystem in weaponSystems)
            {
                foreach (Renderer ren in weaponSystem.GetGameObject().transform.GetComponentsInChildren<Renderer>())
                {
                    ren.enabled = false;
                }
            }
        }
        
        public void ShowWeaponSystems()
        {
            foreach (WeaponSystem weaponSystem in weaponSystems)
            {
                foreach (Renderer ren in weaponSystem.GetGameObject().transform.GetComponentsInChildren<Renderer>())
                {
                    ren.enabled = true;
                }
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