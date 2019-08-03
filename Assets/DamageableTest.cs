using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

public class DamageableTest : MonoBehaviour
{
    [SerializeField] ShipHealth health;
    
    public void ShipDestroyed()
    {
        Debug.Log("Dummy destroyed!");
    }
    public void HullChanged()
    {
        Debug.Log("Hull changed to " + health.hull);
    }
    public void ShieldChanged()
    {
        Debug.Log("Hull changed to " +  health.shield);
    }
}
