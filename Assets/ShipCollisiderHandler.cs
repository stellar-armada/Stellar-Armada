using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using SpaceCommander.Ships;
using UnityEngine;

public class ShipCollisiderHandler : MonoBehaviour, ICollidable
{
    [SerializeField] ShipHealth health;

    public IDamageable GetDamageable()
    {
        return health;
    }
}
