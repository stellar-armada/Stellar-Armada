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
            Debug.Log("Bridge activated!");
            GameObject bridgeObject = Instantiate(bridgePrefab, bridgeRoot);
            bridgeObject.transform.localRotation = Quaternion.identity;
            bridgeObject.transform.localPosition = Vector3.zero;
            localBridge = bridgeObject.GetComponent<ShipBridge>();
            
            // This should be moved to LocalPlayerBridgeSceneRoot and decoupled, maybe
            LocalPlayerBridgeSceneRoot.instance.transform.SetParent(bridgeRoot);
            LocalPlayerBridgeSceneRoot.instance.transform.localPosition = Vector3.zero;
            LocalPlayerBridgeSceneRoot.instance.transform.localRotation = Quaternion.identity;

            if (PlatformManager.instance.Platform == PlatformManager.PlatformType.VR)
            {
                Debug.Log("Setting minimap values here, but this should be decoupled");
                VRMiniMap.instance.transform.SetParent(LocalPlayerBridgeSceneRoot.instance.transform);
                VRMiniMap.instance.transform.localPosition = new Vector3(0, VRMiniMap.instance.yOffset, 0);
                VRMiniMap.instance.transform.localRotation = Quaternion.identity;
            }
            
            // This should be moved to playercontroller and decoupled
            PlayerController.localPlayer.transform.SetParent(LocalPlayerBridgeSceneRoot.instance.transform);
            PlayerController.localPlayer.transform.localPosition = Vector3.zero;
            PlayerController.localPlayer.transform.localRotation = Quaternion.identity;

            localBridge = this;
        }

    }
}