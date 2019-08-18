using Boo.Lang;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public static Placer instance;

    public List<PlacementIndicator> placementIndicators = new List<PlacementIndicator>();
    
    public void ShowPlacements()
    {
        // Get current selection
        
        // Get formation positions for selection
        
        // Foreach ship in selection, get the placer object
    }

    public void HidePlacements()
    {
        
    }

    public void Place()
    {
        
    }
    
    void Awake()
    {
        instance = this;
    }


}
