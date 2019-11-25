using StellarArmada.Levels;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class VRShipPlacementCursor : ShipPlacementCursor
    {
        public Transform transformToFollow;
        public float yOffset = .15f;

        protected override void Initialize()
        {
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