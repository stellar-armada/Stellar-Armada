using SpaceCommander.Teams;

namespace SpaceCommander
{
    // Interface for classes that have the functionality of damaging other players
    public interface IDamager
    {
        IPlayer GetPlayer();
        Team GetTeam();
        void Damage(IDamageable target);
        void SetOwningWeaponSystem(IWeaponSystem weaponSystem);
    }
}