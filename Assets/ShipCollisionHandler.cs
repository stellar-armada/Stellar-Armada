using SpaceCommander.Selection.Tests;
using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class ShipCollisionHandler : MonoBehaviour, ICollidable
    {
        [SerializeField] private ShipHull hull;
        [SerializeField] private ShipShield shield; // Hack til 2019.3 -- replace with idamageable and no need to use both shield and hull
        [SerializeField] IDamageable damageable;
        [SerializeField] TestSelectionHandler selectionHandler;

    void Awake()
    {
        if (hull != null) damageable = hull;
        else  damageable = shield;
    }

    public IDamageable GetDamageable()
    {
        return damageable;
    }

    public ISelectable GetSelectable()
    {
        return selectionHandler;
    }
}
}
