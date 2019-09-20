using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Mirror;
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
       
       float dragDelay = .4f;
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
           t.SetParent(UIPointer.instance.uiPlacerCanvas.transform);
           t.localScale = Vector3.one * scale;
           t.localRotation = Quaternion.identity;
           t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           // disable raycasting on this object
           if(canvasGroup != null) canvasGroup.blocksRaycasts = false;
       }

       public void OnDrag(PointerEventData eventData)
       {
           dragTimer += Time.deltaTime;
           if (dragTimer < dragDelay) return;
           if (isReadyToDrag) Init();
       }

       public void OnEndDrag(PointerEventData eventData)
       {
           if (dragTimer < dragDelay) return;

           GroupContainer g = eventData.pointerCurrentRaycast.gameObject.GetComponent<GroupContainer>();
           
           if (g != null)
           {
               // Shipyard logic
               if (shipyardShip != null)
               {
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
               // return to original group
               if (shipyardShip != null)
               {
                   Shipyard.instance.DestroyShip(shipyardShip);
                  
               }
               else if (groupShip != null)
               {
                   // Group ship logic
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

       public void OnPointerDown(PointerEventData eventData)
       {
          
       }
   }