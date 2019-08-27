using StellarArmada.Levels;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipPlacementCursor : MonoBehaviour
    {
        public static ShipPlacementCursor instance;

        public Transform transformToFollow;

        private Transform t;

        private Vector3 transformToFollowLocalPos;

        public float yOffset = .15f;

        void Awake()
        {
            instance = this;
            t = GetComponent<Transform>();
            transformToFollowLocalPos = transformToFollow.localPosition;

        }

        void Start()
        {
            t.SetParent(MiniMap.instance.transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }

        void LateUpdate()
        {
            Vector3 yOff = transformToFollow.forward *
                           (yOffset * (MiniMap.instance.transform.lossyScale.x / MiniMap.instance.startScale));

            t.position = transformToFollow.position + yOff;
            t.rotation = transformToFollow.rotation;
        }
    }
}