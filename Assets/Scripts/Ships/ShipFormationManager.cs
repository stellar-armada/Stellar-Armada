using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

public enum FormationType
{
    AS_CURRENTLY,
    WALL,
    DELTA,
    
}

[SerializeField]
public struct FormationPoint
{
    public Vector3 position;
    public List<ShipType> preferredShips;
}

public class ShipPositionInfo
{
    public Ship Ship;
    public Vector3 position;
}

public class ShipFormationManager : MonoBehaviour
{
    public List<FormationPoint> formations;

    public List<ShipPositionInfo> GetNormalizedPositionsForShips(List<Ship> ships)
    {
        // Create list of ships and positions
        
        List<ShipPositionInfo> positionTuple = new List<ShipPositionInfo>();

        Vector3 avgPosition = Vector3.zero;
        Vector4 averageRotation = Vector4.zero;

        foreach (Ship ship in ships)
        {
            ShipPositionInfo tuple = new ShipPositionInfo();
            tuple.Ship = ship;
            tuple.position = ship.transform.position;
            // Add to output
            positionTuple.Add(tuple);
            // Average position and rotation of all ships
            avgPosition += tuple.position;
            QuaternionTools.AverageQuaternion(ref averageRotation, ship.transform.rotation, ships[0].transform.rotation, ships.Count);
        }

        avgPosition /= ships.Count;
        
        // Subtract average position from all

        for (int i = 0; i < positionTuple.Count; i++)
        {
            positionTuple[i].position -= avgPosition;
            // Rotate all points inversely to the a average rot (normalize rotation)
            positionTuple[i].position = Quaternion.Inverse(new Quaternion(averageRotation.x, averageRotation.y, averageRotation.z, averageRotation.w)) * positionTuple[i].position;
        }
        
        return positionTuple;
    }
    

}
