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

        [SerializeField] private Transform debugCenterOfGravityTransform;


        [SerializeField] private LayerMask hitPlaneLayerMask;
        [SerializeField] private LayerMask shipLayerMask;

        private Vector2 screenSpaceStartPos = Vector2.zero;
        private Vector2 screenSpaceCurrentPos = Vector2.one;

        [SerializeField] private Transform debugPlacementSphere;
        
        // If the mouse click is very short, set the direction from the CoG of current

        private bool isPlacing = false;

        private Transform shipPlacementCursorTransform;
        
        [SerializeField] private float scaleDistanceMin = 25f;
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

        public override void Place(bool sendToFormation)
        {
            Debug.Log("Placing");
            // Is it a double tap?
                if (Time.time - lastTap < doubleTapThreshold)
                    StopAllShips();
                else
                if(sendToFormation)
                    foreach (ShipPlacementIndicator pi in activePlacements)
                            PlayerController.localPlayer.CmdOrderEntityToMoveToPoint(pi.ship.GetEntityId(), pi.transform.position, pi.transform.rotation);
                else
                    foreach(var selected in ShipSelectionManager.instance.GetCurrentSelection())
                        PlayerController.localPlayer.CmdOrderEntityToMoveToPoint(selected.GetShip().GetEntityId(), shipPlacementCursorTransform.position, shipPlacementCursorTransform.rotation);
                lastTap = Time.time;
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
            // If no ships are selected, skip placement
            if (ShipSelectionManager.instance.GetCurrentSelection().Count < 1)
                return;

            if (Camera.main == null) return;
            
            // If we don't hit the hitplane, return (otherwise, populate hit)
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                    out hit, Mathf.Infinity, hitPlaneLayerMask))
                return;

            screenSpaceCurrentPos = Input.mousePosition;
            currentPos = hit.point;
            
            // Start placement
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                
                // If we hit a ship, return
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, shipLayerMask))
                    return;
                
                RTSCameraController.instance.LockCamera(true);
                ShipFormationManager.instance.scaleXY = ShipFormationManager.instance.minScaleXY;
                ShipFormationManager.instance.scaleZ = ShipFormationManager.instance.minScaleZ;
                // Start raycast
                // Are we hitting the plane?
                startPos = new Vector3(currentPos.x, 0, currentPos.y);
                screenSpaceStartPos = Input.mousePosition;
                thresholdIsSet = false;

                HidePlacements();
                ShipPlacementCursor.instance.Show();
                // If hitting ground plane, set is placing to true
                    // And get and show placements
                    isPlacing = true;
                    // If the mouse is not down, move the cursor to mouse position
                    shipPlacementCursorTransform.position = currentPos;
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
                Place(thresholdIsSet);
                HidePlacements();
                return;

            }
            
            // During placement
            if (Input.GetMouseButton(0))
            {
                if (!isPlacing) return;

                // If we're dragging further than the threshold
                if (Vector2.Distance(screenSpaceCurrentPos, screenSpaceStartPos) > scaleDistanceMin || thresholdIsSet)
                {
                    
                    ShowPlacements();
                    
                    if (!thresholdIsSet)
                        thresholdIsSet = true;
                    
                    currentPos = new Vector3(currentPos.x, 0, currentPos.z);

                    shipPlacementCursorTransform.rotation = Quaternion.LookRotation(Vector3.Normalize(currentPos - startPos), Vector3.up);
                    debugCenterOfGravityTransform.position = CalculateCenterOfGravity();
                    float dist = (Vector2.Distance(screenSpaceCurrentPos, screenSpaceStartPos) - scaleDistanceMin) / (scaleDistanceMax - scaleDistanceMin);

                    ShipFormationManager.instance.scaleXY = Mathf.Lerp(ShipFormationManager.instance.minScaleXY, ShipFormationManager.instance.maxScaleXY, dist);
                    ShipFormationManager.instance.scaleZ = Mathf.Lerp(ShipFormationManager.instance.minScaleZ, ShipFormationManager.instance.maxScaleZ, dist);
                }

                debugPlacementSphere.position = currentPos;
                return;
            }
            
            shipPlacementCursorTransform.localPosition = Vector3.zero;
            debugCenterOfGravityTransform.position = CalculateCenterOfGravity();


        }

        Vector3 CalculateCenterOfGravity()
        {
            List<Ship> ships = new List<Ship>();
    
            foreach (var selectable in ShipSelectionManager.instance.GetCurrentSelection())
                ships.Add(selectable.GetShip());

            return ShipFormationManager.CalculateCenterOfMass(ships);
        }
    }
}