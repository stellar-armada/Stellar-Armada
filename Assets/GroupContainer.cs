using UnityEngine;
using UnityEngine.EventSystems;

public class GroupContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public static GroupContainer currentGroup;
    
    public int groupId;

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentGroup = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentGroup == this) currentGroup = null;
    }
}
