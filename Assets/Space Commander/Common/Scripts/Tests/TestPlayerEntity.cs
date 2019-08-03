using UnityEngine;
namespace SpaceCommander.Common.Tests{
public class TestPlayerEntity : MonoBehaviour, IPlayerEntity
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
}