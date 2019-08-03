namespace SpaceCommander
{
    // Interface for classes that have the functionality of damaging other players
    public interface IWeaponSystemController
    {
        IPlayer GetPlayer();
        IWeaponSystem[] GetWeaponSystems();
    }
}