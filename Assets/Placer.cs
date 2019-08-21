using System.Collections.Generic;
using System.Linq;
using SpaceCommander;
using SpaceCommander.IO;
using SpaceCommander.Ships;
using SpaceCommander.UI;
using UnityEngine;
using Zinnia.Extension;

public class Placer : MonoBehaviour
{
    public static Placer instance;

    public Transform placementPositionRoot;

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
        placementPositionRoot.gameObject.SetActive(!down);
        uiPointerIsActive = down;
    }

    void HandleRightThumbstick(bool down)
    {
        if (leftThumbstickIsDown) return; // if the other button is down, ignore this input
        if (!down && !rightThumbstickIsDown) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
        rightThumbstickIsDown = down;
        placementPositionRoot.gameObject.SetActive(!down);
        uiPointerIsActive = down;
    }

    
    public void ShowPlacements()
    {
        HidePlacements(); // reset our placements for another round of formation calculations
        
        // Set scale of our placer (for backward compatibility with our non-inverse universe scaling method)
        placementPositionRoot.SetGlobalScale(Vector3.one * ScaleManager.GetScale());
        
        // Get current ships
        List<Ship> ships = SelectionUIManager.instance.GetCurrentSelection().Select(selectable => selectable.GetOwningEntity() as Ship).ToList();
        
        // Get formation positions for selection
        var positions = ShipFormationManager.GetFormationPositionsForShips(ships);

        // Foreach ship in selection, get the placer object
        
        foreach (Ship s in ships)
        {
            // Get the placement indicator
            PlacementIndicator pI = PlacementUIManager.placementIndicators.Single(p => p.entity == s);
            
            //Activate
            pI.gameObject.SetActive(true);
            activePlacements.Add(pI); // Quick cache for disabling on placement

            pI.Show(positions[s]);

        }
    }

    public void HidePlacements()
    {
        foreach(PlacementIndicator pI in activePlacements)
            pI.Hide();
        activePlacements = new List<PlacementIndicator>();
    }

    public void Place()
    {
        Debug.Log("Placing!");
        if (uiPointerIsActive) return;
        Transform t = transform;
        foreach (PlacementIndicator pi in activePlacements)
            pi.entity.movement.CmdMoveToPoint(t.position, t.rotation);
    }



}
