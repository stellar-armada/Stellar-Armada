using System.Collections.Generic;
using StellarArmada.Entities;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Levels
{
    public class MiniMapEntity : MonoBehaviour
    {
        public NetworkEntity networkEntity;
        
        private Transform e; // entity ref
        private Transform t; // this transform ref

        [SerializeField] string teamColorMaterialInput;

        [SerializeField] List<Renderer> renderers;

        void Awake()
        {
            t = transform;

            networkEntity.OnTeamChange += HandleTeamChange;
        }

        public void Start()
        {
            e = networkEntity.transform;
            t.SetParent(MiniMap.instance.transform);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
        }

        void HandleTeamChange()
        {
            foreach(Renderer ren in renderers)
            {
                ren.material.SetColor(teamColorMaterialInput, networkEntity.GetTeam().color);
            }
        }
        
        

        void LateUpdate()
        {
            t.localPosition = e.localPosition;
            t.localRotation = e.localRotation;
        }
        
    }
}