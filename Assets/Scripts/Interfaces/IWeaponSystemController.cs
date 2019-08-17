using System.Collections.Generic;

#pragma warning disable 0649
namespace SpaceCommander
{
    // Interface for classes that have the functionality of damaging other players
    public interface IWeaponSystemController
    {
        NetworkEntity GetEntity();
        List<WeaponSystem> GetWeaponSystems();
        void EnableWeaponSystems();
        void DisableWeaponSystems();
        void RegisterWeaponSystem(WeaponSystem weaponSystem);
        bool WeaponSystemsEnabled();
    }
}