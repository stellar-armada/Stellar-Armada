using System.Collections.Generic;
using StellarArmada.Ships;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Levels
{
    public class MiniMapEntity : MonoBehaviour
    {
        [FormerlySerializedAs("Ship")] [FormerlySerializedAs("networkEntity")] public Ship ship;
        
        private Transform e; // entity ref
        private Transform t; // this transform ref

        [SerializeField] string teamColorMaterialInput;

        [SerializeField] List<Renderer> renderers;

        void Awake()
        {
            t = transform;

            ship.OnTeamChange += HandleTeamChange;
            ship.OnEntityDead += Deactivate;
        }

        public void Deactivate()
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = false;
            }
        }

        public void Start()
        {
            e = ship.transform;
            t.SetParent(VRMiniMap.instance.transform);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
        }

        void HandleTeamChange()
        {
            foreach(Renderer ren in renderers)
            {
                ren.material.SetColor(teamColorMaterialInput, ship.GetTeam().color);
            }
        }
        
        

        void LateUpdate()
        {
            t.localPosition = e.localPosition;
            t.localRotation = e.localRotation;
        }
        
    }
}