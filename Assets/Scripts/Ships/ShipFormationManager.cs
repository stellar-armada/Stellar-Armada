using System.Collections.Generic;
using System.Linq;
using StellarArmada.Player;
using UnityEngine;

namespace StellarArmada.Ships
{
    public class ShipFormationManager : MonoBehaviour
    {
        public static ShipFormationManager instance;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            localPlayerControllerTransform = PlayerController.localPlayer.transform;
        }

        private Transform localPlayerControllerTransform;

        private Vector3 lastHitPosition;
        
        public float scaleXY = 600f; // distance between ships next to, above and below
        public float maxScaleXY = 2400f;
        public float minScaleXY = 300f;
        // distance between ships behind/in front
        public float scaleZ = 450f;
        public float minScaleZ = 250f;
        public float maxScaleZ = 2400f;

        public float scaleSpeed = 10f;

        // local reference variables
        protected Dictionary<Ship, Vector3> shipPositions;
        protected Dictionary<Ship, Vector3> offsetShipPositions;

        protected Vector3 centerOfMass;
        protected int count;
        protected List<Ship[]> shipsByLine;
        protected Ship ship;
        int currentFrontlinePosition;
        int currentMidlinePosition;
        int currentBacklinePosition;
        protected Vector3 avgPosition;
        protected Vector4 averageRotation;
        protected float deadZone = .0001f;
        Vector3 centerOfMassForward = Vector3.zero;

        public static Vector3 CalculateCenterOfMass(List<Ship> ships)
        {
            var c = Vector3.zero;
            foreach (Ship s in ships)
            {
                c += s.transform.position;
            }

            return c / ships.Count;
        }
        
        public Dictionary<Ship, Vector3> GetFormationPositionsForShips(List<Ship> ships)
        {
            shipPositions = new Dictionary<Ship, Vector3>();
            // Get center of mass

            centerOfMass = CalculateCenterOfMass(ships);

            centerOfMass /= count;

            // Get rotation of placer
            
            shipsByLine = new List<Ship[]>
            {
                ships.Where(s => s.formationPosition == FormationPosition.Frontline).ToArray(),
                ships.Where(s => s.formationPosition == FormationPosition.Midline).ToArray(),
                ships.Where(s => s.formationPosition == FormationPosition.Backline).ToArray()
            };

            for (int i = shipsByLine.Count; i > 0; i--)
                if (shipsByLine[i - 1].Length == 0)
                {
                    shipsByLine[i - 1] = shipsByLine[i];
                    shipsByLine[i] = new Ship[0];
                }

            // Create positions for ships!
            for (int shipLine = 0; shipLine < shipsByLine.Count; shipLine++)
            {
                int currentPosition = 0;
                while (shipsByLine[shipLine].Length > 0)
                {
                    Vector3 pos = PlatformManager.instance.Platform == PlatformManager.PlatformType.VR ? 
                        FormationMatrices.shipPositionVectors3D[shipLine][currentPosition++] : 
                        FormationMatrices.shipPositionVectors2D[shipLine][currentPosition++];
                    
                    // scale vector up to formation size
                    pos.x *= scaleXY;
                    pos.y *= scaleXY;
                    pos.z *= scaleZ;

                    if (PlatformManager.instance.Platform == PlatformManager.PlatformType.VR)
                        centerOfMassForward = Quaternion.Euler(ShipPlacementCursor.instance.transform.forward) * pos;
                    // 2D - calculate distance by mouse hit
                    else
                        // Handle mobile
                        if (RTSCameraController.instance != null)
                        {
                            Vector3 forward = localPlayerControllerTransform.forward;
                            centerOfMassForward = Quaternion.Euler(forward.x, 0, forward.z) * pos;
                        }

                    // Calculate the position in real space and order ships by distance

                    var lineShips = shipsByLine[shipLine].OrderBy(s =>
                            Vector3.Distance(
                                centerOfMass + centerOfMassForward,
                                s.transform.position))
                        .ToList();

                    ship = lineShips[0];

                    // add to the list of ship positions to return
                    shipPositions.Add(ship, pos);

                    // Remove from the available pool of ships to sort
                    shipsByLine[shipLine] = shipsByLine[shipLine].Where(s => s != ship).ToArray();
                }
            }
            
            
            // Calculate center of gravity of ships in formation and subtract from shippositions

            Vector3 offset = Vector3.zero;

            foreach (var val in shipPositions.Values)
                offset += val;

            // Compute center and offset ships by all
            offset /= shipPositions.Count;

            offsetShipPositions = new Dictionary<Ship, Vector3>();

            
            foreach (var key in shipPositions.Keys)
                offsetShipPositions.Add(key, shipPositions[key] - offset);


            
            return offsetShipPositions;
        }

        public Dictionary<Ship, Vector3> GetFormationPositionWarp(List<Ship> ships)
        {
            shipPositions = new Dictionary<Ship, Vector3>();

            currentFrontlinePosition = 0;
            currentMidlinePosition = 0;
            currentBacklinePosition = 0;

            foreach (Ship s in ships)
            {
                Vector3 pos = Vector3.zero;
                // determine what type of ship it is and return next coordinate
                switch (s.formationPosition)
                {
                    case FormationPosition.Frontline:
                        pos = PlatformManager.instance.Platform == PlatformManager.PlatformType.VR ? 
                            FormationMatrices.shipPositionVectors3D[0][currentFrontlinePosition++] : 
                            FormationMatrices.shipPositionVectors2D[0][currentFrontlinePosition++];
                        break;
                    case FormationPosition.Midline:
                        pos = PlatformManager.instance.Platform == PlatformManager.PlatformType.VR ? 
                            FormationMatrices.shipPositionVectors3D[1][currentMidlinePosition++] : 
                            FormationMatrices.shipPositionVectors2D[1][currentMidlinePosition++];
                        break;
                    case FormationPosition.Backline:
                        pos = PlatformManager.instance.Platform == PlatformManager.PlatformType.VR ? 
                            FormationMatrices.shipPositionVectors3D[2][currentBacklinePosition++] : 
                            FormationMatrices.shipPositionVectors2D[2][currentBacklinePosition++];
                        break;
                }

                // scale vector up to formation size
                pos.x *= scaleXY;
                pos.y *= scaleXY;
                pos.z *= scaleZ;

                // add to dictionary
                shipPositions.Add(s, pos);
            }

            return shipPositions;
        }

        public Dictionary<Ship, Vector3> GetNormalizedPositionsForShips(List<Ship> ships)
        {
            // Create list of ships and positions
            shipPositions = new Dictionary<Ship, Vector3>();

            avgPosition = Vector3.zero;
            averageRotation = Vector4.zero;

            foreach (Ship s in ships)
            {
                shipPositions.Add(s, s.transform.position);
                // Average position and rotation of all ships
                avgPosition += s.transform.position;
                QuaternionTools.AverageQuaternion(ref averageRotation, s.transform.rotation,
                    ships[0].transform.rotation, ships.Count);
            }

            avgPosition /= ships.Count;

            // Subtract average position from all

            foreach (Ship s in shipPositions.Keys)
            {
                shipPositions[s] -= avgPosition;
                // Rotate all points inversely to the a average rot (normalize rotation)
                shipPositions[s] =
                    Quaternion.Inverse(new Quaternion(averageRotation.x, averageRotation.y, averageRotation.z,
                        averageRotation.w)) * shipPositions[s];
            }

            return shipPositions;
        }
    }
}