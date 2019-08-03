using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using UnityEngine;

public class CollisionHandler : MonoBehaviour, ICollidable
{
    public IDamageable owningDamageable;

    public IDamageable GetDamageable()
    {
        return owningDamageable;
    }
}
