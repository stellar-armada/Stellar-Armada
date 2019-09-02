using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Mirror;
   using StellarArmada.UI;
   using UnityEngine;
   using UnityEngine.EventSystems;
   
   public class UIShipDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
   {
       private bool isDragging = false;
       private Vector2 startPosition;
       private RectTransform t;
       private CanvasGroup canvasGroup;
       private UIShipyardShip shipyardShip; // one or the other
       private UIGroupShip groupShip; // one or the other

       public float dragDelay = .25f;
       private float dragTimer = 0f;
       void Awake()
       {
           canvasGroup = GetComponent<CanvasGroup>();
           canvasGroup.blocksRaycasts = true;
           groupShip = GetComponent<UIGroupShip>();
           shipyardShip = GetComponent<UIShipyardShip>();
           t = GetComponent<RectTransform>();
       }
       private PointerEventData data;

       private bool isReadyToDrag = false;
       
       public void OnBeginDrag(PointerEventData eventData)
       {
           dragTimer = 0f;
           isReadyToDrag = true;

       }

       void Init()
       {
           isReadyToDrag = false;
           t.SetParent(UIPointer.instance.uiPlacerCanvas.transform);
           t.localPosition = Vector3.zero;
           t.localScale = Vector3.one;
           t.localRotation = Quaternion.identity;
           t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           isDragging = true;
           // disable raycasting on this object
           canvasGroup.blocksRaycasts = false;
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
           if (GroupContainer.currentGroup)
           {
               Debug.Log("Dropped onto a group, executing group transfer logic");
               // Shipyard logic
               if (shipyardShip != null)
               {
                   Shipyard.instance.MoveShip(shipyardShip, GroupContainer.currentGroup.transform);
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, GroupContainer.currentGroup.groupId);
               }
           }
           else
           {
               Debug.Log("Dropped onto *not* a group, executing group return logic");
               // return to original group
               if (shipyardShip != null)
               {
                   Shipyard.instance.MoveShip(shipyardShip, Shipyard.instance.availableShipsContainer);
                  
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, groupShip.ship.group);
               }
           }
           // enable raycasting on object, maybe
           canvasGroup.blocksRaycasts = true;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0);
           isDragging = false;
       }
   }