using System.Collections.Generic;
using StellarArmada.UI;
   using UnityEngine;
   using UnityEngine.EventSystems;
   
   public class UIShipDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler, IPointerDownHandler
   {
       private Vector2 startPosition;
       private RectTransform t;
       private CanvasGroup canvasGroup;
       private UIShipyardShip shipyardShip; // one or the other
       private UIGroupShip groupShip; // one or the other
       
       float dragDelay = .1f;
       private float dragTimer;
       
       void Awake()
       {
           canvasGroup = GetComponent<CanvasGroup>();
           groupShip = GetComponent<UIGroupShip>();
           shipyardShip = GetComponent<UIShipyardShip>();
           if(shipyardShip != null && canvasGroup != null) canvasGroup.blocksRaycasts = true;
           t = GetComponent<RectTransform>();
       }
       
       private PointerEventData data;

       private bool isReadyToDrag = false;
       private bool isDragging = false;

       private float scale;
       
       public void OnBeginDrag(PointerEventData eventData)
       {
           dragTimer = 0f;
           isReadyToDrag = true;
       }

       void Init()
       {
           isReadyToDrag = false;
           scale = t.localScale.x;

           isDragging = true;

           // TO-DO -- refactor this (could totally be unified logic)
           if (PlatformManager.instance.Platform == PlatformManager.PlatformType.VR)
           {
               t.SetParent(UIPointer.instance.uiPlacerCanvas.transform);
               t.localScale = Vector3.one * scale;
               t.localRotation = Quaternion.identity;
               t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           }
           else
           {
               t.SetParent(PlayerCanvasDraggableLayer.instance.transform);
           }

           // disable raycasting on this object
           if(canvasGroup != null) canvasGroup.blocksRaycasts = false;
       }

       public void OnDrag(PointerEventData eventData)
       {
           dragTimer += Time.deltaTime;
           
           if (isDragging && PlatformManager.instance.Platform != PlatformManager.PlatformType.VR)
           {

               t.position = Input.mousePosition;

           }

           if (dragTimer < dragDelay) return;
           if (isReadyToDrag) Init();
       }

       public void OnEndDrag(PointerEventData eventData)
       {
           if (dragTimer < dragDelay) return;
           
           // TO-DO -- this works for mouse, but probably not VR pointer
           
           PointerEventData pointerData = new PointerEventData (EventSystem.current)
           {
               pointerId = -1,
           };
         
           pointerData.position = Input.mousePosition;
 
           List<RaycastResult> results = new List<RaycastResult>();
           EventSystem.current.RaycastAll(pointerData, results);

           GroupContainer g = null;
               
           // Iterate through all of the hovered objects
               foreach(var e in results)
               {
                   g = e.gameObject.GetComponent<GroupContainer>();
                   if (g) break;
               }

               if (g != null) Debug.Log("g: " + g.name);
               else Debug.Log("g is null");
               
           if (g != null)
           {
               // Shipyard logic
               if (shipyardShip != null)
               {
                   Debug.Log("DragHandler: MoveShip: " + g.groupId);

                   Shipyard.instance.MoveShip(shipyardShip, g);
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, g.groupId);
                   Debug.Log("DragHandler: MoveShipToGroup: " + g.groupId);

               }
           }
           else
           {
               // return to original group
               if (shipyardShip != null)
               {
                   Shipyard.instance.DestroyShip(shipyardShip);
                   Debug.Log("DragHandler: DestroyShip: shipyardShip");
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, groupShip.ship.group);
                   Debug.Log("DragHandler: MoveShipToGroup: groupShip.ship.group");
               }
           }
           // enable raycasting on object, maybe
           if(canvasGroup != null) canvasGroup.blocksRaycasts = true;
           if (PlatformManager.instance.Platform == PlatformManager.PlatformType.VR)
           {
               t.localRotation = Quaternion.identity; 
               t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
               t.localScale = Vector3.one * scale;
           }
           isDragging = false;
       }

       public void OnPointerClick(PointerEventData eventData)
       {
       }

       public void OnPointerDown(PointerEventData eventData)
       {
       }
   }