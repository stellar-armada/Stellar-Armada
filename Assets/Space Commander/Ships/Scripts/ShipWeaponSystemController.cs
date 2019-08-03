using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Weapons;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipWeaponSystemController : MonoBehaviour, IWeaponSystemController
    {
        private IOwnable owner;

        void Awake()
        {
            owner = GetComponent<IOwnable>();
        }
        public IPlayer GetPlayer()
        {
            return owner.GetPlayer();
        }

        public IWeaponSystem[] GetWeaponSystems()
        {
            throw new System.NotImplementedException();
        }
    }
}