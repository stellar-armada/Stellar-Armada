using System.Collections;
using System.Collections.Generic;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShipClickHandler : MonoBehaviour, IPointerClickHandler
{
    private UIShipyardShip shipyardShip;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (shipyardShip != null)
        {
            shipyardShip.SetAsFlagship();
            Debug.Log(gameObject.name + " clicked!");

        }
        
       
    }
}
