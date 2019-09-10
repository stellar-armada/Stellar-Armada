using System.Collections;
using System.Collections.Generic;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShipClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UIShipyardShip shipyardShip;
    [SerializeField] private float maxtime = .2f;

    void Awake()
    {
        shipyardShip = GetComponent<UIShipyardShip>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
    }

    private float timer = 0;
    private bool pointerDown;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (shipyardShip.id < 0) return;
        timer = 0f;
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (shipyardShip.id < 0) return;
        pointerDown = false;
        if (timer < maxtime)
        {
            Shipyard.instance.SetFlagshipForLocalPlayer(shipyardShip);
        }
    }
}
