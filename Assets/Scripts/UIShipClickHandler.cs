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
           Debug.Log("UIShipClickHandler: OnPointerClick attempted");
        if (shipyardShip.id < 0) return;
        Debug.Log("UIShipClickHandler: OnPointerClick");

        int i = PlayerController.localPlayer.GetTeam().prototypes.IndexOf(shipyardShip.GetPrototype());
        PlayerController.localPlayer.CmdSetFlagshipForLocalPlayer(i, PlayerController.localPlayer.netId);
        
    }
}
