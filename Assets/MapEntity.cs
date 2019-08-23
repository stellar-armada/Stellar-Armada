using StellarArmada.Scenarios;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class MapEntity : MonoBehaviour
    {
        public NetworkEntity networkEntity;
        
        private Transform e; // entity ref
        private Transform t; // this transform ref

        void Awake()
        {
            e = networkEntity.transform;
            t = transform;
        }

        public void Start()
        {
            t.SetParent(MiniMap.instance.transform);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
        }

        void LateUpdate()
        {
            t.localPosition = e.localPosition;
            t.localRotation = e.localRotation;
        }
        
    }
}