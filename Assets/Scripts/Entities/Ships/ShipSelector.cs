using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using UnityEngine;
using StellarArmada.Player;
using StellarArmada.UI;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
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

        // Reference to our local player. Serialized so we don't need anti-race logic
        [SerializeField] private PlayerController playerController;

        // Visual indicator of where selection goes
        [SerializeField] Transform selectionCursor;

        [SerializeField] private Renderer selectionCursorRenderer;

        // Radius of the selection query
        public float selectorRadius;

        // Only select entities on these layers
        [SerializeField] LayerMask layerMask;

        public List<ISelectable> currentSelectables = new List<ISelectable>();

        public delegate void SelectorEvent(bool on);

        public SelectorEvent OnHighlightTargetSet;

        public bool targetIsFriendly;

        // State
        private bool isSelecting;
        private bool isDeselecting;
        private bool leftThumbstickIsDown;
        private bool rightThumbstickIsDown;
        private bool leftTriggerIsDown;
        private bool rightTriggerIsDown;
        private bool leftGripIsDown;
        private bool rightGripIsDown;

        // Private reference vars
        private Collider[] hitColliders;
        private ISelectable selectable;
        private float doubleTapThreshold = .5f;
        private float lastTime;

        void Awake() => instance = this;

        void Start()
        {
            InputManager.instance.OnLeftTrigger += HandleLeftTrigger;
            InputManager.instance.OnRightTrigger += HandleRightTrigger;
            InputManager.instance.OnLeftGrip += HandleLeftGrip;
            InputManager.instance.OnRightGrip += HandleRightGrip;
            selectionCursorRenderer.sharedMaterial.SetColor("_BaseColor", ColorManager.instance.defaultColor);
            
        }

        void Update()
        {
            if (MatchPlayerMenuManager.instance.menuIsActive) return;
            
            if (isSelecting)
            {
                Select(SelectionType.Selection);
            }
            
            if (isDeselecting && !isSelecting)
            {
                Select(SelectionType.Deselection);
            }
            
            if (!isSelecting)
            {
                Highlight();
            }
        }

        void HandleLeftTrigger(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
            if (MatchPlayerMenuManager.instance.menuIsActive) return;
            if (rightTriggerIsDown) return; // if the other button is down, ignore this input
            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (!down && !leftTriggerIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftTriggerIsDown = down;
            HandleTrigger(down);
   
        }

        void HandleRightTrigger(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            if (MatchPlayerMenuManager.instance.menuIsActive) return;
            if (leftTriggerIsDown) return; // if the other button is down, ignore this input
            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (!down && !rightTriggerIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightTriggerIsDown = down;
            HandleTrigger(down);
        }

        void HandleTrigger(bool down)
        {
            if (down) StartSelection();
            else EndSelection();
        }

        void HandleDoubleTap()
        {
            ShipSelectionManager.instance.ClearSelection();
        }

        void HandleLeftGrip(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
            if (MatchPlayerMenuManager.instance.menuIsActive) return;
            if (isSelecting) return; // we're already selecting, so we don't want to start a deselection
            if (rightGripIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftGripIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftGripIsDown = down;
            HandleGrip(down);
        }

        void HandleRightGrip(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return; // If this isn't the current hand, ignore input
            if (isSelecting) return; // we're already selecting, so we don't want to start a deselection
            if (MatchPlayerMenuManager.instance.menuIsActive) return;
            if (leftGripIsDown) return; // if the other button is down, ignore this input
            if (!down && !rightGripIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightGripIsDown = down;
            HandleGrip(down);
        }

        void HandleGrip(bool down)
        {
            if (down)
            {
                if (Time.time - lastTime < doubleTapThreshold)
                {
                    HandleDoubleTap();
                }

                lastTime = Time.time;
            }

            if (down) StartDeselection();
            else EndDeselection();
        }


        void Highlight()
        {
            hitColliders = Physics.OverlapSphere(selectionCursor.position, selectorRadius, layerMask);

            List<ISelectable> highlightedSelectables = new List<ISelectable>();
            // Get list of hits
            foreach (var collider in hitColliders)
            {
                selectable = collider.GetComponent<ISelectable>();
                if (selectable != null) highlightedSelectables.Add(selectable);
            }
            
            // Unhilight any from last selectoin that are no longer selected
            List<ISelectable> unhighlightSelectables = currentSelectables.Where(c => !highlightedSelectables.Contains(c)).ToList();

            foreach (var selectable in unhighlightSelectables)
                selectable.Unhighlight();
            
            // If currentselectables contains ships that aren't on the list, deselect
            currentSelectables = currentSelectables.Where(c => highlightedSelectables.Contains(c)).ToList();
            
            // if currentselectables doesnt contain any ships on the list, select
            foreach (var selectable in highlightedSelectables)
            {
                if(!currentSelectables.Contains(selectable)) currentSelectables.Add(selectable );
            }

            if (currentSelectables.Count == 0)
            {
                selectionCursorRenderer.sharedMaterial.SetColor("_BaseColor", ColorManager.instance.defaultColor);
                UIPointer.instance.SetColor(ColorManager.instance.defaultColor);
                return;
            }
            
            foreach (var selectable in currentSelectables)
            {
                // Friendly
                    if (selectable.GetOwningEntity().GetTeam() == playerController.GetTeam())
                    {
                        targetIsFriendly = true;
                        selectable.Highlight(ColorManager.instance.friendlyColor);
                        selectionCursorRenderer.sharedMaterial.SetColor("_BaseColor", ColorManager.instance.friendlyColor);
                        UIPointer.instance.SetColor(ColorManager.instance.friendlyColor);
                    }
                    // Enemy
                    else if (selectable != null && selectable.GetOwningEntity().GetTeam() != playerController.GetTeam())
                    {
                        targetIsFriendly = false;
                        selectable.Highlight(ColorManager.instance.enemyColor);
                        selectionCursorRenderer.sharedMaterial.SetColor("_BaseColor", ColorManager.instance.enemyColor);
                        UIPointer.instance.SetColor(ColorManager.instance.enemyColor);
                    }
            }
            OnHighlightTargetSet?.Invoke(true);
        }

        // Handles selection and deselection of entities from the loop
        public void Select(SelectionType selectionType)
        {
            // Query nearby colliders
            hitColliders = Physics.OverlapSphere(selectionCursor.position, selectorRadius, layerMask);
            foreach (Collider collider in hitColliders)
            {
                // Try getting a selectable from the collider
                selectable = collider.GetComponent<ISelectable>();
                if (selectable != null && selectable.IsSelectable() &&
                    selectable.GetOwningEntity().GetTeam() == playerController.GetTeam())
                {
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
                else
                {
                    Debug.LogError("Failed to select");
                }
            }
        }

        public void StartSelection()
        {
            isSelecting = true;
        }

        public void EndSelection() => isSelecting = false;

        public void StartDeselection()
        {
            isDeselecting = true;
        }

        public void EndDeselection() => isDeselecting = false;
    }
}