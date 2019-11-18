using StellarArmada.IO;
using UnityEngine;
using StellarArmada.Player;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    // Handles local player entity selection
    // TO-DO: Abstract button-specifics to input manager
    // TO-DO: MenuIsActive / UIPointerIsActive might need to be checks to the singleton class
    // TO-DO: Scale selection cursor size with map scale
    // TO-DO: Clean up overly-complex logic that manages handedness, if inputmanager handles handedness then we can cut this code down a lot
    // TO-DO: Handle select single and clear selection
    // TO-DO: Raycast vs brush selection
    
    public class ShipSelector : MonoBehaviour
    {
        // Reference to our local player. Serialized so we don't need anti-race logic
        [SerializeField] protected PlayerController playerController;
        
        public delegate void SelectorEvent(bool on);

        public SelectorEvent OnHighlightTargetSet;

        public bool targetIsFriendly;

        // Private reference vars
        protected float doubleTapThreshold = .5f;
        protected float lastTime;
        
        // Only select entities on these layers
        [SerializeField] protected LayerMask layerMask;
        
        protected Collider[] hitColliders;
        public List<ISelectable> currentSelectables = new List<ISelectable>();


        // State
        protected bool isSelecting;
        protected bool isDeselecting;
        
        void Awake() => instance = this;

        protected void StartSelection()
        {
            isSelecting = true;
            Select(SelectionType.Selection);
        }

        protected void EndSelection() => isSelecting = false;

        protected void StartDeselection()
        {
            isDeselecting = true;
            Select(SelectionType.Deselection);
        }

        protected void EndDeselection() => isDeselecting = false;
    }
}