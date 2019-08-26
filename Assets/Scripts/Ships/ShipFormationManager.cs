using System.Collections.Generic;
using System.Linq;
using StellarArmada.Ships;
using UnityEngine;
using VRKeys;

public class ShipFormationManager : MonoBehaviour
{
    public static ShipFormationManager instance;

    public float scaleXY = 350f;
    public float scaleZ = 250f;
    private Dictionary<Ship, Vector3> shipPositions;
    void Awake()
    {
        instance = this;
    }
    public Dictionary<Ship, Vector3> GetFormationPositionsForShips(List<Ship> ships)
    {
        shipPositions = new Dictionary<Ship, Vector3>();
        
        
        
        // Get center of mass

        Vector3 centerOfMass = Vector3.zero;
        int count = 0;
        foreach (Ship s in ships)
        {
            centerOfMass += s.transform.position;
            count++;
        }

        centerOfMass /= count;
        
        // Get rotation of placer


        Ship[][] shipsByLine = new Ship[3][] {
            ships.Where(s => s.formationPosition == Ship.FormationPosition.Frontline).ToArray(),
            ships.Where(s => s.formationPosition == Ship.FormationPosition.Midline).ToArray(),
            ships.Where(s => s.formationPosition == Ship.FormationPosition.Backline).ToArray()
        };
        
        for (int shipLine = 0; shipLine < shipsByLine.Length; shipLine++)
        {
            int currentPosition = 0;
            while(shipsByLine[shipLine].Length > 0)
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
                            Quaternion.Euler(PlacementCursor.instance.transform.forward) * pos, s.transform.position)).ToList();
                
                Ship ship = lineShips[0];

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

        int currentFrontlinePosition = 0;
        int currentMidlinePosition = 0;
        int currentBacklinePosition = 0;

        foreach (Ship s in ships)
        {
            Vector3 pos = Vector3.zero;
            // determine what type of ship it is and return next coordinate
            switch (s.formationPosition)
            {
                case Ship.FormationPosition.Frontline:
                    pos = FormationMatrices.shipPositionVectors[0][currentFrontlinePosition++];
                    break;
                case Ship.FormationPosition.Midline:
                    pos = FormationMatrices.shipPositionVectors[1][currentMidlinePosition++];
                    break;
                case Ship.FormationPosition.Backline:
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

    public static Dictionary<Ship, Vector3> GetNormalizedPositionsForShips(List<Ship> ships)
    {
        // Create list of ships and positions
        Dictionary<Ship, Vector3> shipPositions = new Dictionary<Ship, Vector3>();
        
        Vector3 avgPosition = Vector3.zero;
        Vector4 averageRotation = Vector4.zero;

        foreach (Ship ship in ships)
        {
            shipPositions.Add(ship, ship.transform.position);
            // Average position and rotation of all ships
            avgPosition += ship.transform.position;
            QuaternionTools.AverageQuaternion(ref averageRotation, ship.transform.rotation, ships[0].transform.rotation, ships.Count);
        }

        avgPosition /= ships.Count;
        
        // Subtract average position from all

        foreach (Ship s in shipPositions.Keys)
        {
            shipPositions[s] -= avgPosition;
            // Rotate all points inversely to the a average rot (normalize rotation)
            shipPositions[s] = Quaternion.Inverse(new Quaternion(averageRotation.x, averageRotation.y, averageRotation.z, averageRotation.w)) * shipPositions[s];
        }
        
        return shipPositions;
    }
    
    
    

}
