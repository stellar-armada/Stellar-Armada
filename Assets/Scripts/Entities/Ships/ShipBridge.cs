using StellarArmada.Player;
using UnityEngine;

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
            GameObject bridgeObject = GameObject.Instantiate(bridgePrefab, bridgeRoot);
            bridgeObject.transform.localRotation = Quaternion.identity;
            bridgeObject.transform.localPosition = Vector3.one;
            localBridge = bridgeObject.GetComponent<ShipBridge>();
            HumanPlayerController.localPlayer.transform.SetParent(bridgeRoot);
            ship.availableAsFlagship = false;
        }

    }
}