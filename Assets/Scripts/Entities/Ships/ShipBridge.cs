using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipBridge : MonoBehaviour
    {
        [SerializeField] private Ship ship;
        public GameObject bridgePrefab;
        public Transform bridgeRoot;

        public static ShipBridge localBridge;

        public void ActivateBridgeForLocalPlayer()
        {
            GameObject bridgeObject = Instantiate(bridgePrefab, bridgeRoot);
            bridgeObject.transform.localRotation = Quaternion.identity;
            bridgeObject.transform.localPosition = Vector3.zero;
            localBridge = bridgeObject.GetComponent<ShipBridge>();
            
            LocalPlayerBridgeSceneRoot.instance.transform.SetParent(bridgeRoot);
            LocalPlayerBridgeSceneRoot.instance.transform.localPosition = Vector3.zero;
            LocalPlayerBridgeSceneRoot.instance.transform.localRotation = Quaternion.identity;
            
            MiniMap.instance.transform.SetParent(LocalPlayerBridgeSceneRoot.instance.transform);
            MiniMap.instance.transform.localPosition = new Vector3(0, MiniMap.instance.yOffset, 0);
            MiniMap.instance.transform.localRotation = Quaternion.identity;

            HumanPlayerController.localPlayer.transform.SetParent(LocalPlayerBridgeSceneRoot.instance.transform);
            HumanPlayerController.localPlayer.transform.localPosition = Vector3.zero;
            HumanPlayerController.localPlayer.transform.localRotation = Quaternion.identity;

            localBridge = this;
        }

    }
}