using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

public class FormationVectorData
{
    public static List<Vector3> frontlineVectors = new List<Vector3>
    {
        // Frontline vectors
        new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(0,1,0),
            new Vector3(0,-1,0),
            new Vector3(1,1,0),
            new Vector3(-1,1,0),
            new Vector3(1,-1,0),
            new Vector3(-1,-1,0),
            new Vector3(2,0,0),
            new Vector3(-2,0,0),
            new Vector3(0,2,0),
            new Vector3(0,-2,0),
            new Vector3(2,1,0),
            new Vector3(-2,1,0),
            new Vector3(2,-1,0),
            new Vector3(-2,-1,0),
            new Vector3(1,2,0),
            new Vector3(-1,2,0),
            new Vector3(1,-2,0),
            new Vector3(-1,-2,0)
    } ;
    
    public static List<Vector3> midlineVectors = new List<Vector3>()
    {
        new Vector3(.5f,0,-1),
        new Vector3(-.5f,0,-1),
        new Vector3(0,.5f,-1),
        new Vector3(0,-.5f,-1),
        new Vector3(1, .5f, -1),
        new Vector3(-1, .5f, -1),
        new Vector3(1, -.5f, -1),
        new Vector3(-1, -.5f, -1),
        new Vector3(0,1.5f,-1),
        new Vector3(0,-1.5f,-1),
        new Vector3(1.5f,0,-1),
        new Vector3(-1.5f,0,-1),
        new Vector3(1.5f,1.5f,-1),
        new Vector3(-1.5f,1.5f,-1),
        new Vector3(1.5f,-1.5f,-1),
        new Vector3(-1.5f,-1.5f,-1),
    };
    
    public static List<Vector3> backlineVectors = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(1,1,0),
        new Vector3(-1,1,0),
        new Vector3(1,-1,0),
        new Vector3(-1,-1,0),
        new Vector3(2,0,0),
        new Vector3(-2,0,0),
        new Vector3(0,2,0),
        new Vector3(0,-2,0),
        new Vector3(2,1,0),
        new Vector3(-2,1,0),
        new Vector3(2,-1,0),
        new Vector3(-2,-1,0),
        new Vector3(1,2,0),
        new Vector3(-1,2,0),
        new Vector3(1,-2,0),
        new Vector3(-1,-2,0)
    } ;
}

public class ShipFormationManager : MonoBehaviour
{
    public static ShipFormationManager instance;

    public float scaleXY = 350f;
    public float scaleZ = 250f;
    
    void Awake()
    {
        instance = this;
    }

    public static Dictionary<Ship, Vector3> GetFormationPositionsForShips(List<Ship> ships)
    {
        Dictionary<Ship, Vector3> shipPositions = new Dictionary<Ship, Vector3>();

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
                    pos = FormationVectorData.frontlineVectors[currentFrontlinePosition++];
                    break;
                case Ship.FormationPosition.Midline:
                    pos = FormationVectorData.midlineVectors[currentMidlinePosition++];
                    break;
                case Ship.FormationPosition.Backline:
                    pos = FormationVectorData.backlineVectors[currentBacklinePosition++];
                    break;
            }
            
            // scale vector up to formation size
            pos.x *= instance.scaleXY;
            pos.y *= instance.scaleXY;
            pos.z *= instance.scaleZ;
            
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
