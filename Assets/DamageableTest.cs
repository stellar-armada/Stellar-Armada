using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using SpaceCommander.Ships;
using SpaceCommander.Teams;
using UnityEngine;

public class DamageableTest : MonoBehaviour, IPlayer
{
    [SerializeField] ShipHealth health;

    [SerializeField] private bool isEnemy;

    void Awake()
    {
        health.SetOwnable(GetComponent<IPlayerOwnedEntity>());
    }
    
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

    public bool IsLocalPlayer()
    {
        throw new System.NotImplementedException();
    }

    public bool IsServer()
    {
        throw new System.NotImplementedException();
    }

    public bool IsClient()
    {
        throw new System.NotImplementedException();
    }

    public void RegisterPlayer()
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterPlayer()
    {
        throw new System.NotImplementedException();
    }

    public PlayerType GetPlayerType()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Team GetTeam()
    {
        Team t = new Team {color = Color.black, teamID = 100, name = "TestTeam"};
        return t;
    }

    public uint GetId()
    {
        throw new System.NotImplementedException();
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public bool IsEnemy(IPlayer player)
    {
        return isEnemy;
    }
}
