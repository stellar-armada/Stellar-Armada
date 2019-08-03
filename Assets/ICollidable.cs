namespace SpaceCommander
{
    // Interface for collision handlers (i.e. scripts attached to colliders) that might not be directly on the IDamageable object
    // All damageables need a ICollidable reference to be detected by the turret system
    public interface ICollidable
    {
        IDamageable GetDamageable();
    }
}