using StellarArmada.Selection;

#pragma warning disable 0649
namespace StellarArmada
{
    public interface ISelector
    {
        void Select(SelectionType selectionType);
        void StartSelection();
        void EndSelection();
        void StartDeselection();
        void EndDeselection();
    }
}