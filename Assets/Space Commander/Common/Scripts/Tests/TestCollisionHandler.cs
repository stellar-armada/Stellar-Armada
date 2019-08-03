using SpaceCommander.Selection.Tests;
using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class TestCollisionHandler : MonoBehaviour, ICollidable
{
    [SerializeField] TestDamageable health;
    [SerializeField] TestSelectionHandler selectionHandler;

    public IDamageable GetDamageable()
    {
        return health;
    }

    public ISelectable GetSelectable()
    {
        return selectionHandler;
    }
}
}
