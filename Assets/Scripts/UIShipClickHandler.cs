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

        int i = PlayerController.localPlayer.GetTeam().prototypes.IndexOf(shipyardShip.GetPrototype());
        PlayerController.localPlayer.CmdSetFlagshipForLocalPlayer(i, PlayerController.localPlayer.netId);
        
    }
}
