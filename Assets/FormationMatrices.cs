using System.Collections.Generic;
using UnityEngine;

public class FormationMatrices
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
