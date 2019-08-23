#pragma warning disable 0649
namespace StellarArmada
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
        NetworkEntity GetOwningEntity();
        bool IsSelectable();
    }
}
