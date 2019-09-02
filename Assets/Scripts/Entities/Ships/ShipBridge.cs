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
            bridgeObject.transform.localPosition = Vector3.one;
            localBridge = bridgeObject.GetComponent<ShipBridge>();
            SceneRoot.instance.transform.SetParent(bridgeRoot);
            HumanPlayerController.localPlayer.transform.SetParent(SceneRoot.instance.transform);
        }

    }
}