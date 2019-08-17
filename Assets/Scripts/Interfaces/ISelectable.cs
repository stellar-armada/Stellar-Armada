#pragma warning disable 0649
namespace SpaceCommander
{
    public interface ISelectable
    {
        int GetSelectionSetID();
        void SetSelectionSetID(int id);
        void Select();
        void Deselect();
        NetworkEntity GetOwningEntity();
        bool IsSelectable();
    }
}
