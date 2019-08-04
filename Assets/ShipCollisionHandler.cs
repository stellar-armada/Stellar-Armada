using SpaceCommander.Selection;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceCommander.Ships
{
    public class ShipCollisionHandler : MonoBehaviour, ICollidable
{
    [FormerlySerializedAs("health")] [SerializeField] ShipHull hull;
    [SerializeField] ShipSelectionHandler selectionHandler;

    public IDamageable GetDamageable()
    {
        return hull;
    }

    public SpaceCommander.ISelectable GetSelectable()
    {
        return selectionHandler;
    }
}
}
