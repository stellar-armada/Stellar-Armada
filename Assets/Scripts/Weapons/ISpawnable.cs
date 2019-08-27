#pragma warning disable 0649
namespace StellarArmada
{
    // Interface for handling spawnables
    // (Note: spawning is currently handled by SendMessage, so OnSpawned isn't called directly and might be greyed out in your IDE)
    public interface ISpawnable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
