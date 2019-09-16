using System.Collections.Generic;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityWeaponSystemController : NetworkBehaviour, IWeaponSystemController
    {
        public Transform currentTarget;
        public bool targetIsFriendly;
                
        [SerializeField] private NetworkEntity entity;
        
        List<WeaponSystem> weaponSystems = new List<WeaponSystem>();

        public bool weaponSystemsEnabled = false;

        [Server]
        public void ServerSetTarget(uint entityId, bool isFriendly)
        {
            if(isServerOnly) SetTarget(entityId, isFriendly);
            RpcSetTarget(entityId, isFriendly);
        }

        [ClientRpc]
        void RpcSetTarget(uint entityId, bool isFriendly)
        {
            SetTarget(entityId, isFriendly);
        }

        void SetTarget(uint entityId, bool isFriendly)
        {
            currentTarget = EntityManager.GetEntityById(entityId).transform;
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