using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Mirror;
   using StellarArmada.UI;
   using UnityEngine;
   using UnityEngine.EventSystems;
   
   public class UIShipDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
   {
       private Vector2 startPosition;
       private RectTransform t;
       private CanvasGroup canvasGroup;
       private UIShipyardShip shipyardShip; // one or the other
       private UIGroupShip groupShip; // one or the other

       private bool hasDelay;
       
       float dragDelay = .4f;
       private float dragTimer;
       
       void Awake()
       {
           canvasGroup = GetComponent<CanvasGroup>();
           groupShip = GetComponent<UIGroupShip>();
           shipyardShip = GetComponent<UIShipyardShip>();
           // if (shipyardShip != null) hasDelay = true;
           t = GetComponent<RectTransform>();
       }
       
       private PointerEventData data;

       private bool isReadyToDrag = false;

       private float scale;
       
       public void OnBeginDrag(PointerEventData eventData)
       {
           Debug.Log("Begin drag called");
           if (hasDelay)
           {
               
           dragTimer = 0f;
           isReadyToDrag = true;
           }
           else
           {
               InitDrag();
           }
       }

       void InitDrag()
       {
           Debug.Log("Init drag called");
           isReadyToDrag = false;
           scale = t.localScale.x;
           t.SetParent(UIPointer.instance.uiPlacerCanvas.transform);
           t.localScale = Vector3.one * scale;
           t.localRotation = Quaternion.identity;
           t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           // disable raycasting on this object
           if(canvasGroup != null) canvasGroup.blocksRaycasts = false;
       }

       public void OnDrag(PointerEventData eventData)
       {
           Debug.Log("On drag called");
           dragTimer += Time.deltaTime;
           if (dragTimer < dragDelay) return;
           InitDrag();
       }

       public void OnEndDrag(PointerEventData eventData)
       {
           Debug.Log("On drag called");
           if (hasDelay && dragTimer < dragDelay) return;
           if (eventData.pointerCurrentRaycast.gameObject.GetComponent<GroupContainer>() != null)
           {
               Debug.Log("*** Group container not null: " + eventData.pointerCurrentRaycast.gameObject.name);
           }

           GroupContainer g = eventData.pointerCurrentRaycast.gameObject.GetComponent<GroupContainer>();
           
           if (g != null)
           {
               Debug.Log("Dropped onto a group, executing group transfer logic");
               // Shipyard logic
               if (shipyardShip != null)
               {
                   Debug.Log("Moving shipyard ship");
                   Shipyard.instance.MoveShip(shipyardShip, g);
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, g.groupId);
               }
           }
           else
           {
               Debug.Log("Dropped onto *not* a group, executing group return logic");
               // return to original group
               if (shipyardShip != null)
               {
                   Debug.Log("Destroying shipyard ship shipyard ship");
                   Shipyard.instance.DestroyShip(shipyardShip);
                  
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   Debug.Log("Moving Group ship back");
                   GroupUIManager.instance.MoveShipToGroup(groupShip, groupShip.ship.group);
               }
           }
           // enable raycasting on object, maybe
           if(canvasGroup != null) canvasGroup.blocksRaycasts = true;
           t.localRotation = Quaternion.identity;
           t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           t.localScale = Vector3.one * scale;
       }

       public void OnPointerClick(PointerEventData eventData)
       {
           
       }
   }