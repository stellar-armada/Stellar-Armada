using SpaceCommander.Game;
using UnityEngine;
namespace SpaceCommander.Common.Tests{
public class TestPlayerEntity : MonoBehaviour, IPlayerEntity
{
    private IPlayer testPlayer;

    public void SetPlayer(IPlayer player)
    {
        testPlayer = player;
    }
    
    public void CmdSetPlayer(uint playerId)
    {
        testPlayer = PlayerManager.GetPlayerByNetId(playerId);
    }

    public uint GetEntityId()
    {
        return 0;
        //TO-DO: Figure out how our entity registration works...
    }

    public IPlayer GetPlayer()
    {
        return testPlayer;
    }
}
}