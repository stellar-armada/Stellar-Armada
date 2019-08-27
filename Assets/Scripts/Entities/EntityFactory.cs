using Mirror;

namespace StellarArmada.Entities
{
    // Base class for factories that create entities
    public abstract class EntityFactory : NetworkBehaviour
    {
        // Share a unique incrementer amongst all entities for dictionary lookups
        public static uint entityIncrement = 0;
    }
}