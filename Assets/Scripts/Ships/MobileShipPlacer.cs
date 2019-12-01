using System.Collections.Generic;
using System.Linq;
using StellarArmada.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StellarArmada.Ships
{
#pragma warning disable 0649
    public class MobileShipPlacer : ShipPlacer
    {
        private Camera cam;
        
        RaycastHit hit;

        [SerializeField] private LayerMask layerMask;

        // To-do -- this could be more performant
        [SerializeField] private string hitPlaneLayerName = "RaycastPlane";

        private Vector2 screenSpaceStartPos = Vector2.zero;
        private Vector2 screenSpaceCurrentPos = Vector2.one;
        
        // If the mouse click is very short, set the direction from the CoG of current

        private bool isPlacing = false;

        private Transform shipPlacementCursorTransform;
        
        [SerializeField] private float scaleDistanceMin = 50f;
        [SerializeField] private float scaleDistanceMax = 500f;

        void Start()
        {
            shipPlacementCursorTransform = ShipPlacementCursor.instance.transform;
            ShipFormationManager.instance.scaleXY = ShipFormationManager.instance.minScaleXY;
            ShipFormationManager.instance.scaleZ = ShipFormationManager.instance.minScaleZ;
        }

        public override void ShowPlacements()
        {
            HidePlacements(); // reset our placements for another round of formation calculations
            
            // Get current ships
            List<Ship> ships = ShipSelectionManager.instance.GetCurrentSelection()
                .Select(selectable => selectable.GetShip()).ToList();

            // Get formation positions for selection
            var positions = ShipFormationManager.instance.GetFormationPositionsForShips(ships);
            
            foreach (Ship s in ships)
            {
                // Get the placement indicator
                ShipPlacementIndicator pI = ShipPlacementUIManager.placementIndicators.Single(p => p.ship == s);

                //Activate
                pI.gameObject.SetActive(true);
                activePlacements.Add(pI); // Quick cache for disabling on placement
                pI.transform.SetParent(shipPlacementCursorTransform, true);
                pI.transform.localScale = Vector3.one;
                pI.transform.localRotation = Quaternion.identity;
                pI.Show(positions[s]);
            }
        }

        public override void Place()
        {
            Debug.Log("Placing");
            // If we're not highlighting a ship
            if (ShipSelector.instance.currentSelectables.Count == 0)
            {
                // Is it a double tap?
                if (Time.time - lastTap < doubleTapThreshold)
                    StopAllShips();
                else
                    foreach (ShipPlacementIndicator pi in activePlacements)
                        PlayerController.localPlayer.CmdOrderEntityToMoveToPoint(pi.ship.GetEntityId(), pi.transform.position, pi.transform.rotation);
                lastTap = Time.time;
            }
            // We are highlighting a ship, so pursue it
            else
            {
                foreach (ShipPlacementIndicator pi in activePlacements)
                    PlayerController.localPlayer.CmdOrderEntityToPursue(pi.ship.GetEntityId(),
                        ShipSelector.instance.currentSelectables[0].GetShip().GetEntityId(), ShipSelector.instance.targetIsFriendly);
            }
        }

        protected override void HidePlacements()
        {

            foreach (ShipPlacementIndicator pI in activePlacements)
                pI.Hide();
            activePlacements = new List<ShipPlacementIndicator>();
        }

        private Vector3 startPos;
        private Vector3 currentPos;

        private bool thresholdIsSet = false;
        void Update()
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                    out hit, Mathf.Infinity, layerMask) || hit.transform.gameObject.layer != LayerMask.NameToLayer(hitPlaneLayerName) )
                return;
            
            screenSpaceCurrentPos = Input.mousePosition;
            currentPos = hit.point;
            
            // Start placement
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                RTSCameraController.instance.LockCamera(true);
                ShipFormationManager.instance.scaleXY = ShipFormationManager.instance.minScaleXY;
                ShipFormationManager.instance.scaleZ = ShipFormationManager.instance.minScaleZ;
                // Start raycast
                // Are we hitting the plane?
                startPos = new Vector3(currentPos.x, 0, currentPos.y);
                screenSpaceStartPos = Input.mousePosition;
                thresholdIsSet = false;

                Vector3 shipCoG = CalculateCenterOfGravity();
                shipCoG = new Vector3(shipCoG.x, 0, shipCoG.z);
                    shipPlacementCursorTransform.rotation = Quaternion.LookRotation(Vector3.Normalize(startPos - shipCoG), Vector3.up);
                
                // If hitting ground plane, set is placing to true
                    // And get and show placements
                    isPlacing = true;
                    ShowPlacements();
                    ShipPlacementCursor.instance.Show();
                    return;
            }
            
            // End placement
            if (Input.GetMouseButtonUp(0))
            {
                if (!isPlacing) return;
                ShipPlacementCursor.instance.Hide();
                RTSCameraController.instance.LockCamera(false);

                // Get and show placements
                isPlacing = false;
                Place();
                HidePlacements();
                return;

            } 
            
            // During placement
            if (Input.GetMouseButton(0))
            {
                if (!isPlacing) return;

                if (Vector2.Distance(screenSpaceCurrentPos, screenSpaceStartPos) > scaleDistanceMin || thresholdIsSet)
                {
                    if (!thresholdIsSet)
                        thresholdIsSet = true;
                    
                    currentPos = new Vector3(currentPos.x, 0, currentPos.z);

                    shipPlacementCursorTransform.rotation = Quaternion.LookRotation(Vector3.Normalize(currentPos - startPos), Vector3.up);
                    
                    float dist = (Vector2.Distance(screenSpaceCurrentPos, screenSpaceStartPos) - scaleDistanceMin) / (scaleDistanceMax - scaleDistanceMin);

                    ShipFormationManager.instance.scaleXY = Mathf.Lerp(ShipFormationManager.instance.minScaleXY, ShipFormationManager.instance.maxScaleXY, dist);
                    ShipFormationManager.instance.scaleZ = Mathf.Lerp(ShipFormationManager.instance.minScaleZ, ShipFormationManager.instance.maxScaleZ, dist);
                }
                else
                {
                    Vector3 shipCoG = CalculateCenterOfGravity();
                    shipCoG = new Vector3(shipCoG.x, 0, shipCoG.z);
                    shipPlacementCursorTransform.rotation = Quaternion.LookRotation(Vector3.Normalize(startPos - shipCoG), Vector3.up);
                }
                ShowPlacements();
                return;
            }
            
            // If the mouse is not down, move the cursor to mouse position
            shipPlacementCursorTransform.position = currentPos;
        }

        Vector3 CalculateCenterOfGravity()
        {
            List<Ship> ships = new List<Ship>();
            foreach (var placement in activePlacements)
            {
                ships.Add(placement.ship);
            }

            return ShipFormationManager.CalculateCenterOfMass(ships);
        }
    }
}