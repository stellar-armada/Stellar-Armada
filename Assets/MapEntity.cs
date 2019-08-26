using System.Collections.Generic;
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

        [SerializeField] string teamColorMaterialInput;

        private MaterialPropertyBlock m;
        [SerializeField] List<Renderer> renderers;

        void Awake()
        {
            e = networkEntity.transform;
            t = transform;
            m = new MaterialPropertyBlock();
        }

        public void Start()
        {
            t.SetParent(MiniMap.instance.transform);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            // Set team color

            foreach(Renderer ren in renderers)
            {
               ren.GetPropertyBlock(m);
               m.SetColor(teamColorMaterialInput, networkEntity.GetTeam().color);
               ren.SetPropertyBlock(m);
            }
            
        }

        void LateUpdate()
        {
            t.localPosition = e.localPosition;
            t.localRotation = e.localRotation;
        }
        
    }
}