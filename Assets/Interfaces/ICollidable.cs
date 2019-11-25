#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Interface for collision handlers (i.e. scripts attached to colliders) that might not be directly on the IDamageable or ISelectable object this interface references
    // All damageables need a ICollidable reference to be detected by the turret system
    // All selectables need an ICollidable reference to be detected by the selection system
    public interface ICollidable
    {
        IDamageable GetDamageable();
    }
}