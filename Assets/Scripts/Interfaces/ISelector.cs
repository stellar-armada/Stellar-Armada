#pragma warning disable 0649
namespace SpaceCommander.Selection
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