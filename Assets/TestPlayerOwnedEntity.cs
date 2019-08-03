using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using SpaceCommander.Ships.Tests;
using UnityEngine;

public class TestPlayerOwnedEntity : MonoBehaviour, IPlayerOwnedEntity
{
    private TestPlayer testPlayer;

    public void SetPlayer(TestPlayer player)
    {
        testPlayer = player;
    }
    
    public void CmdSetPlayer(uint playerID)
    {
        throw new System.NotImplementedException();
    }

    public uint GetId()
    {
        throw new System.NotImplementedException();
    }

    public IPlayer GetPlayer()
    {
        return testPlayer;
    }
}
