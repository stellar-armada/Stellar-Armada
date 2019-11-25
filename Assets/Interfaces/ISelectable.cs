using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Interface for selecting objects that reference an entity
    public interface ISelectable
    {
        void Select();
        void Deselect();
        Ship GetShip();
        bool IsSelectable();
        void Highlight(Color highlightColor);
        void Unhighlight();
    }
}
