using System.Collections.Generic;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Interface for objects that manage an entities' weapon systems
    // ShipWeaponSystemController, for example
    public interface IWeaponSystemController
    {
        Ship GetEntity();
        List<WeaponSystem> GetWeaponSystems();
        void EnableWeaponSystems();
        void DisableWeaponSystems();
        void RegisterWeaponSystem(WeaponSystem weaponSystem);
        bool WeaponSystemsEnabled();
    }
}