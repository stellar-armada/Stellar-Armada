using SpaceCommander.Selection;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipCollisionHandler : MonoBehaviour, ICollidable
{
    [SerializeField] ShipHealth health;
    [SerializeField] ShipSelectionHandler selectionHandler;

    public IDamageable GetDamageable()
    {
        return health;
    }

    public SpaceCommander.ISelectable GetSelectable()
    {
        return selectionHandler;
    }
}
}
