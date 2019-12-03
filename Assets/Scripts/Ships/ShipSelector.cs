using System.Collections.Generic;
using UnityEngine;
using StellarArmada.Player;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Handles local player entity selectionuiPointerIsActive
    // TO-DO: Abstract button-specifics to input manager
    // TO-DO: MenuIsActive / UIPointerIsActive might need to be checks to the singleton class
    // TO-DO: Scale selection cursor size with map scale
    // TO-DO: Clean up overly-complex logic that manages handedness, if inputmanager handles handedness then we can cut this code down a lot
    // TO-DO: Handle select single and clear selection
    // TO-DO: Raycast vs brush selection

    public class ShipSelector : MonoBehaviour
    {
        public static ShipSelector instance;
        
        protected virtual void Awake() => instance = this;

        public delegate void SelectorEvent(bool on);

        public SelectorEvent OnHighlightTargetSet;

        [HideInInspector] public bool targetIsFriendly;

        // Private reference vars
        protected float doubleTapThreshold = .5f;
        protected float lastTime;
        
        // Only select entities on these layers
        [SerializeField] protected LayerMask layerMask;
        
        protected Collider[] hitColliders;
        public List<ISelectable> currentSelectables = new List<ISelectable>();
    }
}