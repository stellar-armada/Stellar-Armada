using System.Collections.Generic;
using StellarArmada.Ships;
using UnityEngine;

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
                    pos = FormationMatrices.frontlineVectors[currentFrontlinePosition++];
                    break;
                case Ship.FormationPosition.Midline:
                    pos = FormationMatrices.midlineVectors[currentMidlinePosition++];
                    break;
                case Ship.FormationPosition.Backline:
                    pos = FormationMatrices.backlineVectors[currentBacklinePosition++];
                    break;
            }
            
            Debug.Log("Formation manager: " + instance);
            
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
