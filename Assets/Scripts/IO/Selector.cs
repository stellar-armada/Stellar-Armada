using SpaceCommander.Selection;
using SpaceCommander.UI;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.IO
{
    public class Selector : MonoBehaviour, ISelector
    {
        [SerializeField] Transform cursor;
        [SerializeField] UIPointer pointer;

        public float selectorRadius;
        
        [SerializeField] LayerMask layerMask;

        private bool isSelecting;
        private bool isDeselecting;

        private bool uiPointerIsActive;

        [SerializeField] private PlayerController playerController;

        void Awake()
        {
            pointer.OnCanvasStateChanged += HandlePointerOnCanvas;
            
        }

        void Start()
        {
            InputManager.instance.OnLeftTrigger += (on) =>
            {
                if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
                if(on) StartSelection();
                else EndSelection();
            };
            
            InputManager.instance.OnRightTrigger += (on) =>
            {
                if (!HandSwitcher.instance.CurrentHandIsRight()) return;
                if(on) StartSelection();
                else EndSelection();
            };
            
            InputManager.instance.OnLeftGrip += (on) =>
            {
                if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
                if(on) StartDeselection();
                else EndDeselection();
            };
            
            InputManager.instance.OnRightGrip += (on) =>
            {
                if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
                if(on) StartDeselection();
                else EndDeselection();
            };
        }
        
        

        void HandlePointerOnCanvas(bool pointerIsRaycastingCanvas)
        {
            cursor.gameObject.SetActive(!pointerIsRaycastingCanvas);
            uiPointerIsActive = pointerIsRaycastingCanvas;
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
            Collider[] hitColliders = Physics.OverlapSphere(cursor.position, selectorRadius, layerMask);
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
        }

        public void EndSelection()
        {
            isSelecting = false;
        }

        public void StartDeselection()
        {
            isDeselecting = true;
        }

        public void EndDeselection()
        {
            isDeselecting = false;
        }
    }
}