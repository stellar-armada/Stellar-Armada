using System.Collections.Generic;

namespace SpaceCommander
{
    // Interface for classes that have the functionality of damaging other players
    public interface IWeaponSystemController
    {
        IPlayerOwnedEntity GetOwner();
        void SetOwner(IPlayerOwnedEntity owner);
        List<IWeaponSystem> GetWeaponSystems();
        void EnableWeaponSystems();
        void DisableWeaponSystems();
        void RegisterWeaponSystem(WeaponSystem weaponSystem);
        bool WeaponSystemsEnabled();
    }
}