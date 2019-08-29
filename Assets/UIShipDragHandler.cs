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
           canvasGroup.blocksRaycasts = false;
           groupShip = GetComponent<UIGroupShip>();
           shipyardShip = GetComponent<UIShipyardShip>();
           t = GetComponent<RectTransform>();
       }
   
       public static GameObject itemBeingDragged;
       private PointerEventData data;
   
       private GroupContainer startContainer;
       
       public void OnBeginDrag(PointerEventData eventData)
       {

           data = new PointerEventData(EventSystem.current);
           itemBeingDragged = gameObject;
           startPosition = t.position;
           isDragging = true;
           // disable raycasting on this object
           canvasGroup.blocksRaycasts = false;
           Debug.Log("Setting start container");
           startContainer = GroupContainer.currentGroup;
           if(shipyardShip!=null) shipyardShip.transform.SetParent(startContainer.GetComponentInParent<Canvas>().transform);
           else if (groupShip != null) groupShip.transform.SetParent(startContainer.GetComponentInParent<Canvas>().transform);
               dragTimer = 0f;
       }
   
       public void OnDrag(PointerEventData eventData)
       {
           dragTimer += Time.deltaTime;
           if (dragTimer < dragDelay) return;
           data = new PointerEventData(EventSystem.current);
           t.position = data.position;
           
       }

       public void OnEndDrag(PointerEventData eventData)
       {
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
                   Shipyard.instance.MoveShip(shipyardShip, startContainer.transform);
               }
               else if (groupShip != null)
               {
                   // Group ship logic
                   GroupUIManager.instance.MoveShipToGroup(groupShip, startContainer.groupId);
               }
           }
           // enable raycasting on object, maybe
           canvasGroup.blocksRaycasts = true;

           itemBeingDragged = null;
           isDragging = false;
       }
   }