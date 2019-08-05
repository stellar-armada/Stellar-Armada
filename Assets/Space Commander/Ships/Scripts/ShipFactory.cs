using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Ships;
using UnityEngine;

[System.Serializable]
public class ShipDictionary : SerializableDictionary<ShipType, GameObject>
{
}

public class ShipFactory : NetworkBehaviour
{
    public static ShipFactory instance;

    [SerializeField] ShipDictionary ships = new ShipDictionary();

    void Awake()
    {
        instance = this;
    }
// Ship Factory
    [Command]
    public void CmdCreateShipForPlayer(uint playerID, ShipType shipType, Vector3 position, Quaternion rotation)
    {
        GameObject shipPrefab;
        
        bool success = ships.TryGetValue(shipType, out shipPrefab);
        if (!success)
        {
            Debug.Log("Failed to create ship - was not found in dictionary");
            return;
        }
        
        GameObject shipGameObject = Instantiate(shipPrefab, position, rotation, SceneRoot.instance.transform);
        
        NetworkServer.Spawn(shipGameObject);
        
        Ship s = shipGameObject.GetComponent<Ship>();
        
        s.CmdSetPlayer(playerID);
    }
    
    // Ship Factory
    [Command]
    public void CmdCreateShipForTeam(uint teamId, int groupId, ShipType shipType, Vector3 position, Quaternion rotation)
    {
        GameObject shipPrefab;
        
        bool success = ships.TryGetValue(shipType, out shipPrefab);
        if (!success)
        {
            Debug.Log("Failed to create ship - was not found in dictionary");
            return;
        }
        
        GameObject shipGameObject = Instantiate(shipPrefab, position, rotation, SceneRoot.instance.transform);
        
        NetworkServer.Spawn(shipGameObject);
        
        Ship s = shipGameObject.GetComponent<Ship>();

        s.CmdSetTeam(teamId);
        s.CmdSetGroup(groupId);
    }
}
