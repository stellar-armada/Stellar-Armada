using System.Collections.Generic;
using System.Linq;
using SpaceCommander;
using SpaceCommander.Ships;
using SpaceCommander.UI;
using UnityEngine;
using Zinnia.Extension;

public class Placer : MonoBehaviour
{
    public static Placer instance;

    public Transform placementPositionRoot;

    private List<PlacementIndicator> activePlacements = new List<PlacementIndicator>();

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
            PlacementIndicator pI = PlacementManager.placementIndicators.Single(p => p.entity == s);
            
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
        Transform t = transform;
        foreach (PlacementIndicator pi in activePlacements)
            pi.entity.movement.MoveToPoint(t.position, t.rotation);
    }
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SelectionUIManager.instance.OnSelectionChanged += ShowPlacements;
    }


}
