using System.Collections.Generic;
using System.Linq;
using StellarArmada;
using StellarArmada.IO;
using StellarArmada.Ships;
using StellarArmada.UI;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public static Placer instance;
    
    private List<PlacementIndicator> activePlacements = new List<PlacementIndicator>();

    private bool uiPointerIsActive;
        
    private bool leftThumbstickIsDown;
    private bool rightThumbstickIsDown;

    private bool rightPlaceButtonIsDown;
    private bool leftPlaceButtonIsDown;
    
        
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SelectionUIManager.instance.OnSelectionChanged += ShowPlacements;
        
        // Thumbsticks show hide our placementPositionRoot
        InputManager.instance.OnLeftThumbstickButton += HandleLeftThumbstick;
        InputManager.instance.OnRightThumbstickButton += HandleRightThumbstick;

        InputManager.instance.OnButtonOne += HandleRightPlaceButton;
        InputManager.instance.OnButtonThree += HandleLeftPlaceButton;

    }

    void HandleRightPlaceButton(bool down)
    {
        if (!HandSwitcher.instance.CurrentHandIsRight()) return;
        if (leftPlaceButtonIsDown) return; // if the other button is down, ignore this input
        if (!down && !rightPlaceButtonIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
        rightPlaceButtonIsDown = down;
        if (down) Place();
    }

    void HandleLeftPlaceButton(bool down)
    {
        if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
        if (rightPlaceButtonIsDown) return; // if the other button is down, ignore this input
        if (!down && !leftPlaceButtonIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
        leftPlaceButtonIsDown = down;
        if (down) Place();
    }

    void HandleLeftThumbstick(bool down)
    {
        if (rightThumbstickIsDown) return; // if the other button is down, ignore this input
        if (!down && !leftThumbstickIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
        leftThumbstickIsDown = down;
        PlacementCursor.instance.gameObject.SetActive(!down);
        uiPointerIsActive = down;
    }

    void HandleRightThumbstick(bool down)
    {
        if (leftThumbstickIsDown) return; // if the other button is down, ignore this input
        if (!down && !rightThumbstickIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
        rightThumbstickIsDown = down;
        PlacementCursor.instance.gameObject.SetActive(!down);
        uiPointerIsActive = down;
    }

    
    public void ShowPlacements()
    {
        if(IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));
        
        HidePlacements(); // reset our placements for another round of formation calculations

        // Get current ships
        List<Ship> ships = SelectionUIManager.instance.GetCurrentSelection().Select(selectable => selectable.GetOwningEntity() as Ship).ToList();

        // Get formation positions for selection
        var positions = ShipFormationManager.instance.GetFormationPositionsForShips(ships);

        // Foreach ship in selection, get the placer object
        
        foreach (Ship s in ships)
        {
            // Get the placement indicator
            PlacementIndicator pI = PlacementUIManager.placementIndicators.Single(p => p.entity == s);
            
            //Activate
            pI.gameObject.SetActive(true);
            activePlacements.Add(pI); // Quick cache for disabling on placement
            pI.transform.SetParent(PlacementCursor.instance.transform, true);
            pI.transform.localScale = Vector3.one;
            pI.transform.localRotation = Quaternion.identity;
            pI.Show(positions[s]);
            Invoke(nameof(ShowPlacements), .2f);
        }
    }

    public void HidePlacements()
    {
        if(IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));
        foreach(PlacementIndicator pI in activePlacements)
            pI.Hide();
        activePlacements = new List<PlacementIndicator>();
    }

    public void Place()
    {
        if (uiPointerIsActive) return;

        foreach (PlacementIndicator pi in activePlacements)
        {
            // For each placer, set the minimap parent so we can grab local pos and rot easier
            // Otherwise we could optimise with some matrix/quaternion math!
            pi.transform.SetParent(MiniMap.instance.transform, true);
            pi.entity.movement.CmdMoveToPoint(pi.transform.localPosition, pi.transform.localRotation);
            pi.transform.SetParent(PlacementCursor.instance.transform, true);
            
        }
    }



}
