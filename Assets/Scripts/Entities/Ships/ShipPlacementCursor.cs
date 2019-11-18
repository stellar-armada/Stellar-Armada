using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipPlacementCursor : MonoBehaviour
    {
        public static ShipPlacementCursor instance;

        public Transform transformToFollow;

        private Transform t;
        
        public float yOffset = .15f;

        private bool isInitialized = false;

        [SerializeField] private PlayerController playerController;
        void Awake()
        {
            instance = this;
            t = GetComponent<Transform>();
            playerController.OnLocalPlayerInitialized += Initialize;
        }

        void Initialize()
        {
            Debug.Log("<color=green>Initialized placement cursor</color>");
            isInitialized = true;
            t.SetParent(VRMiniMap.instance.transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }

        void LateUpdate()
        {
            if (!isInitialized) return;
            Vector3 yOff = transformToFollow.forward * (yOffset * (VRMiniMap.instance.transform.lossyScale.x / VRMiniMap.instance.startScale));

            t.position = transformToFollow.position + yOff;
            t.rotation = transformToFollow.rotation;
        }
    }
}