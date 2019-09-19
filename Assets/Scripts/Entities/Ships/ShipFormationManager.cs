using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
// Local player singleton that manages how ships group together when warping in or being placed by selection
    public class ShipFormationManager : MonoBehaviour
    {
        public static ShipFormationManager instance; // singleton accessor

        public float scaleXY = 600f; // distance between ships next to, above and below
        public float maxScaleXY = 2400f;
        public float minScaleXY = 300f;

        // distance between ships behind/in front
        public float scaleZ = 450f;
        public float minScaleZ = 250f;
        public float maxScaleZ = 2400f;

        public float scaleSpeed = 10f;

        // local reference variables
        private Dictionary<Ship, Vector3> shipPositions;
        private Vector3 centerOfMass;
        private int count;
        private Ship[][] shipsByLine;
        private Ship ship;
        int currentFrontlinePosition;
        int currentMidlinePosition;
        int currentBacklinePosition;
        private Vector3 avgPosition;
        private Vector4 averageRotation;

        private float deadZone = .0001f;

        void Start()
        {
            InputManager.instance.OnLeftThumbstickAnalog += (direction) =>
            {
                if (HandSwitcher.instance.CurrentHandIsLeft()) dPad = direction;
            };
            InputManager.instance.OnRightThumbstickAnalog += (direction) =>
            {
                if (HandSwitcher.instance.CurrentHandIsRight()) dPad = direction;
            };
        }

        private Vector2 dPad = Vector2.zero;

        void HandleThumbstick()
        {
            
            if (Mathf.Abs(dPad.x) > deadZone) // dPad X value is above deadzone
            {
                float newXY = scaleXY + dPad.x * scaleSpeed * Time.deltaTime;
                scaleXY = Mathf.Clamp(newXY, minScaleXY, maxScaleXY);
            }
            
            if (Mathf.Abs(dPad.y) > deadZone) // dPad Y value is above deadzone
            {
                float newZ = scaleZ + dPad.y * scaleSpeed * Time.deltaTime;
                scaleZ = Mathf.Clamp(newZ, minScaleZ, maxScaleZ);
            }
        }

        void Update()
        {
            HandleThumbstick();
        }
        
        void Awake()
        {
            instance = this;
        }

        public Dictionary<Ship, Vector3> GetFormationPositionsForShips(List<Ship> ships)
        {
            shipPositions = new Dictionary<Ship, Vector3>();
            // Get center of mass

            centerOfMass = Vector3.zero;
            count = 0;
            foreach (Ship s in ships)
            {
                centerOfMass += s.transform.position;
                count++;
            }

            centerOfMass /= count;

            // Get rotation of placer


            shipsByLine = new Ship[3][]
            {
                ships.Where(s => s.formationPosition == FormationPosition.Frontline).ToArray(),
                ships.Where(s => s.formationPosition == FormationPosition.Midline).ToArray(),
                ships.Where(s => s.formationPosition == FormationPosition.Backline).ToArray()
            };

            // Some hardcoded recursive logic to organize the ships toward the back and minimize gaps
            
            // if row 2 is empty, move row 1 to 2
            if (shipsByLine[1].Length == 0)
            {
                shipsByLine[1] = shipsByLine[0];
                shipsByLine[0] = new Ship[0];
            }
            
            // if row 3 is empty, move both up
            if (shipsByLine[2].Length == 0)
            {
                shipsByLine[2] = shipsByLine[1];
                shipsByLine[1] = shipsByLine[0];
                shipsByLine[0] = new Ship[0];
            }

            // Create positions for ships!
            for (int shipLine = 0; shipLine < shipsByLine.Length; shipLine++)
            {
                int currentPosition = 0;
                while (shipsByLine[shipLine].Length > 0)
                {
                    Vector3 pos = FormationMatrices.shipPositionVectors[shipLine][currentPosition++];
                    // scale vector up to formation size
                    pos.x *= scaleXY;
                    pos.y *= scaleXY;
                    pos.z *= scaleZ;

                    // Calculate the position in real space and order ships by distance

                    var lineShips = shipsByLine[shipLine].OrderBy(s =>
                            Vector3.Distance(
                                centerOfMass +
                                Quaternion.Euler(ShipPlacementCursor.instance.transform.forward) * pos,
                                s.transform.position))
                        .ToList();

                    ship = lineShips[0];

                    // add to the list of ship positions to return
                    shipPositions.Add(ship, pos);

                    // Remove from the available pool of ships to sort
                    shipsByLine[shipLine] = shipsByLine[shipLine].Where(s => s != ship).ToArray();
                }
            }

            return shipPositions;
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
                        pos = FormationMatrices.shipPositionVectors[0][currentFrontlinePosition++];
                        break;
                    case FormationPosition.Midline:
                        pos = FormationMatrices.shipPositionVectors[1][currentMidlinePosition++];
                        break;
                    case FormationPosition.Backline:
                        pos = FormationMatrices.shipPositionVectors[2][currentBacklinePosition++];
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