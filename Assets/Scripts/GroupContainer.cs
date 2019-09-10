using StellarArmada.UI;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class GroupContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public static GroupContainer currentGroup;
    
    public int groupId;

    [SerializeField] private bool onlyNull;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onlyNull)
        {
            currentGroup = null;
            return;
        }
        currentGroup = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // var raycast = eventData.pointerCurrentRaycast.gameObject;
    //    if (!raycast.GetComponent<UIShipyardShip>() && !raycast.GetComponent<UIGroupShip>() && currentGroup == this) currentGroup = null;
    }
}
