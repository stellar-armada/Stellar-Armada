using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipWeaponSystemController : NetworkBehaviour, IWeaponSystemController
    {
        public Transform currentTarget;
        public bool targetIsFriendly;
                
        [FormerlySerializedAs("entity")] [SerializeField] private Ship ship;
        
        List<WeaponSystem> weaponSystems = new List<WeaponSystem>();

        public bool weaponSystemsEnabled = false;

        [Server]
        public void ServerSetTarget(uint entityId, bool isFriendly)
        {
            if(isServer) SetTarget(entityId, isFriendly);
            RpcSetTarget(entityId, isFriendly);
        }

        [ClientRpc]
        void RpcSetTarget(uint entityId, bool isFriendly)
        {
            SetTarget(entityId, isFriendly);
        }

        void SetTarget(uint entityId, bool isFriendly)
        {
            currentTarget = ShipManager.GetEntityById(entityId).transform;
            targetIsFriendly = isFriendly;
            
            foreach (WeaponSystem ws in weaponSystems)
            {
                if (ws.target == currentTarget) continue;
                if (ws.targetsFriendlies && isFriendly) ws.target = currentTarget;
                if (!ws.targetsFriendlies && !isFriendly) ws.target = currentTarget;
            }
        }

        public void ClearTargets()
        {
            currentTarget = null;
            foreach (WeaponSystem ws in weaponSystems)
            {
                ws.ClearTarget();
            }
        }
        
        public Ship GetEntity()
        {
            return ship;
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