using StellarArmada.Selection.Tests;
using StellarArmada.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Common.Tests
{
    public class TestCollisionHandler : MonoBehaviour, ICollidable
    {
        [SerializeField] private TestDamageableHull hull;
        [SerializeField] private TestDamageableShield shield; // Hack til 2019.3
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
