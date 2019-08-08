namespace SpaceCommander
{
    public interface ISelectable
    {
        int GetSelectionSetID();
        void SetSelectionSetID(int id);
        void Select();
        void Deselect();
        IEntity GetOwningEntity();
        bool IsSelectable();
    }
}
