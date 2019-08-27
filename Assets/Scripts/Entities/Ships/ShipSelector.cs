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
        [SerializeField] private PlayerController playerController;

        // Visual indicator of where selection goes
        [SerializeField] Transform selectionCursor;
        
        // Radius of the selection query
        public float selectorRadius;
        
        // Only select entities on these layers
        [SerializeField] LayerMask layerMask; 
        
        // State
        private bool isSelecting;
        private bool isDeselecting;
        private bool uiPointerIsActive;
        private bool leftThumbstickIsDown;
        private bool rightThumbstickIsDown;
        private bool leftTriggerIsDown;
        private bool rightTriggerIsDown;
        private bool leftGripIsDown;
        private bool rightGripIsDown;

        // Private reference vars
        private ShipSelectionManager _shipSelectionManager;

        private Collider[] hitColliders;
        private ISelectable selectable;
        
        void Start()
        {
            // Subscribe to delegates in start to avoid race condition with singleton
            InputManager.instance.OnLeftThumbstickButton += HandleLeftThumbstick;
            InputManager.instance.OnRightThumbstickButton += HandleRightThumbstick;
            InputManager.instance.OnLeftTrigger += HandleLeftTrigger;
            InputManager.instance.OnRightTrigger += HandleRightTrigger;
            InputManager.instance.OnLeftGrip += HandleLeftGrip;
            InputManager.instance.OnRightGrip += HandleRightGrip;

            _shipSelectionManager = ShipSelectionManager.instance;
        }
        
        void Update()
        {
            if (uiPointerIsActive) return; // If UI is active, selection is disabled
            if (isSelecting)
            {
                Select(SelectionType.Selection);
            } else if (isDeselecting)
            {
                Select(SelectionType.Deselection);
            }
        }
        
        void HandleLeftThumbstick(bool down)
        {
            if (rightThumbstickIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftThumbstickIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftThumbstickIsDown = down;
            selectionCursor.gameObject.SetActive(!down);
            uiPointerIsActive = down;
        }

        void HandleRightThumbstick(bool down)
        {
            if (leftThumbstickIsDown) return; // if the other button is down, ignore this input
            if (!down && !rightThumbstickIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightThumbstickIsDown = down;
            selectionCursor.gameObject.SetActive(!down);
            uiPointerIsActive = down;
        }
        
        void HandleLeftTrigger(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
            if (rightTriggerIsDown) return; // if the other button is down, ignore this input

            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (!down && !leftTriggerIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftTriggerIsDown = down;
            if(down) StartSelection();
            else EndSelection();
        }

        void HandleRightTrigger(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            if (leftTriggerIsDown) return; // if the other button is down, ignore this input

            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (!down && !rightTriggerIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightTriggerIsDown = down;
            if(down)StartSelection();
            else EndSelection();
        }
        
        void HandleLeftGrip(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
            if (isSelecting) return; // we're already selecting, so we don't want to start a deselection
            if (rightGripIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftGripIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftGripIsDown = down;
            if(down) StartDeselection();
            else EndDeselection();
        }

        void HandleRightGrip(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return; // If this isn't the current hand, ignore input
            if (isSelecting) return; // we're already selecting, so we don't want to start a deselection
            if (leftGripIsDown) return; // if the other button is down, ignore this input
            if (!down && !rightGripIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightGripIsDown = down;
            if(down) StartDeselection();
            else EndDeselection();
        }
        
        // Handles selection and deselection of entities from the loop
        public void Select(SelectionType selectionType)
        {            
            // Query nearby colliders
            hitColliders = Physics.OverlapSphere(selectionCursor.position, selectorRadius, layerMask);
            foreach(Collider collider in hitColliders)
            {
                // Try getting a selectable from the collider
                selectable = collider.GetComponent<ISelectable>();
                // Has a selectable component, so it's an entity
                if (selectable != null)
                {
                    if (!selectable.IsSelectable()) return; // The selectable can't be selected right now
                    if (selectable.GetOwningEntity().GetTeam().teamId != playerController.teamId) return; // The selectable isn't on our team, could be wrapped into IsSelectable query
                    
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            ShipSelectionManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            ShipSelectionManager.instance.RemoveFromSelection(selectable);
                            break;
                    }
                }
            } 
        }

        public void StartSelection()
        {
            isSelecting = true;
            Select(SelectionType.Selection);
        }

        public void EndSelection() => isSelecting = false;

        public void StartDeselection()
        {
            isDeselecting = true;
            Select(SelectionType.Deselection);
        }

        public void EndDeselection() => isDeselecting = false;
    }
}