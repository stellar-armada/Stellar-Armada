namespace SpaceCommander
{
    // Interface for classes that have the functionality of damaging other players
    public interface IDamager
    { void Damage(IDamageable target);
        void SetOwningWeaponSystem(IWeaponSystem weaponSystem);
    }
}