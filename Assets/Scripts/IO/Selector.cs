using SpaceCommander.Selection;
using SpaceCommander.UI;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.IO
{
    public class Selector : MonoBehaviour, ISelector
    {
        [SerializeField] Transform selectionCursor;

        public float selectorRadius;
        
        [SerializeField] LayerMask layerMask;

        private bool isSelecting;
        private bool isDeselecting;

        private bool uiPointerIsActive;
        
        private bool leftThumbstickIsDown;
        private bool rightThumbstickIsDown;
        
        private bool leftTriggerIsDown;
        private bool rightTriggerIsDown;
        
        private bool leftGripIsDown;
        private bool rightGripIsDown;
        

        [SerializeField] private PlayerController playerController;

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
            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (rightTriggerIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftTriggerIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftTriggerIsDown = down;
            if(down) StartSelection();
            else EndSelection();
        }

        void HandleRightTrigger(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            if (isDeselecting) return; // we're already deselection, so we don't want to start a selection
            if (leftTriggerIsDown) return; // if the other button is down, ignore this input
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
        
        void Start()
        {
            InputManager.instance.OnLeftThumbstickButton += HandleLeftThumbstick;
            InputManager.instance.OnRightThumbstickButton += HandleRightThumbstick;

            InputManager.instance.OnLeftTrigger += HandleLeftTrigger;
            InputManager.instance.OnRightTrigger += HandleRightTrigger;
            InputManager.instance.OnLeftGrip += HandleLeftGrip;
            InputManager.instance.OnRightGrip += HandleRightGrip;
        }
        
        void Update()
        {
            if (uiPointerIsActive) return;
            if (isSelecting)
            {
                Select(SelectionType.Selection);
            } else if (isDeselecting)
            {
                Select(SelectionType.Deselection);
            }
        }

        public void Select(SelectionType selectionType)
        {            
            Collider[] hitColliders = Physics.OverlapSphere(selectionCursor.position, selectorRadius, layerMask);
            foreach(Collider collider in hitColliders)
            {
                ICollidable collidable = collider.GetComponent<ICollidable>();
                if (collidable != null)
                {
                    ISelectable selectable = collidable.GetSelectable();
                    if (!selectable.IsSelectable()) return;
                    if (selectable.GetOwningEntity().GetTeam().teamId != playerController.teamId) return;
                    
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            SelectionUIManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            SelectionUIManager.instance.RemoveFromSelection(selectable);
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

        public void EndSelection()
        {
            isSelecting = false;
        }

        public void StartDeselection()
        {
            isDeselecting = true;
            Select(SelectionType.Deselection);
        }

        public void EndDeselection()
        {
            isDeselecting = false;
        }
    }
}