using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkData : NetworkBehaviour
{
    public static NetworkData instance;
/*
    [SyncVar] private AnchorPair azureSpatialAnchorNumber;    
    
    public AnchorPair GetCurrentSpatialAnchor() => azureSpatialAnchorNumber;

    [Command]
    public void CmdSetSpacialAnchor(AnchorPair val)
    {
        azureSpatialAnchorNumber = val;
    }
    */
    
    void Awake()
    {
        if(instance != null) Debug.LogError("NetworkData instance is not null");
        instance = this;
    } 
}
