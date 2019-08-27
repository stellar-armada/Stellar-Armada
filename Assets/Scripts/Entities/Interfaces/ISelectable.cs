#pragma warning disable 0649
namespace StellarArmada.Entities
{
    // Interface for selecting objects that reference an entity
    public interface ISelectable
    {
        void Select();
        void Deselect();
        NetworkEntity GetOwningEntity();
        bool IsSelectable();
    }
}
