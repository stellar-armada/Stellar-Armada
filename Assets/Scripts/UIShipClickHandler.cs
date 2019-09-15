using System.Collections;
using System.Collections.Generic;
using StellarArmada.Player;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShipClickHandler : MonoBehaviour, IPointerClickHandler
{
    private UIShipyardShip shipyardShip;

    void Awake()
    {
        shipyardShip = GetComponent<UIShipyardShip>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shipyardShip.id < 0) return;
        int i = HumanPlayerController.localPlayer.GetTeam().prototypes.IndexOf(shipyardShip.GetPrototype());
        HumanPlayerController.localPlayer.CmdSetFlagshipForLocalPlayer(i, HumanPlayerController.localPlayer.netId);
    }
}
