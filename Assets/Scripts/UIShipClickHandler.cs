using System.Collections;
using System.Collections.Generic;
using StellarArmada.Player;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShipClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UIShipyardShip shipyardShip;
    private float maxtime = .3f;

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
        Debug.Log("<color=blue>CLICK</color> OnPointerDown called");
        if (shipyardShip.id < 0) return;
        timer = 0f;
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("<color=orange>CLICK</color> OnPointerUp called");
        if (!pointerDown)
        {
            Debug.Log("<color=red>CLICK</color> OnPointerUp called, but failed because pointerDown wasn't true");
            return;
        }
        if (shipyardShip.id < 0)
        {
            Debug.Log("<color=red>CLICK</color> OnPointerUp called, but failed because ship ID wasn't set");
            return;
        }
        pointerDown = false;
        if (timer < maxtime)
        {
            Debug.Log("<color=blue>CLICK</color> OnPointerUp called successfully");

            // Get the list index of the ship we want to set the captain to
            int i = HumanPlayerController.localPlayer.GetTeam().prototypes.IndexOf(shipyardShip.GetPrototype());
            HumanPlayerController.localPlayer.CmdSetFlagshipForLocalPlayer(i, HumanPlayerController.localPlayer.netId);
        }
    }
}
