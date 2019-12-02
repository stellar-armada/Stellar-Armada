using System.Collections.Generic;
using UnityEngine;

namespace StellarArmada.Ships
{
    public class FormationMatrices
    {
        
        public static List<List<Vector3>> shipPositionVectors2D = new List<List<Vector3>>
        {
            new List<Vector3>
            {
                // Frontline vectors
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(-2, 0, 0),
                new Vector3(3, 0, 0),
                new Vector3(-3, 0, 0),
                new Vector3(4, 0, 0),
                new Vector3(-4, 0, 0),
                new Vector3(5, 0, 0),
                new Vector3(-5, 0, 0),
                new Vector3(6, 0, 0),
                new Vector3(-6, 0, 0),
                new Vector3(7, 0, 0),
                new Vector3(-7, 0, 0),
                new Vector3(8, 0, 0),
                new Vector3(-8, 0, 0),
                new Vector3(9, 0, 0),
                new Vector3(-9, 0, 0),
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 0),
                new Vector3(11, 0, 0),
                new Vector3(-11, 0, 0)
            },
            new List<Vector3>
            {
                new Vector3(.5f, 0, -1),
                new Vector3(-.5f, 0, -1),
                new Vector3(1.5f, 0, -1),
                new Vector3(-1.5f, 0, -1),
                new Vector3(2.5f, 0, -1),
                new Vector3(-2.5f, 0, -1),
                new Vector3(3.5f, 0, -1),
                new Vector3(-3.5f, 0, -1),
                new Vector3(4.5f, 0, -1),
                new Vector3(-4.5f, 0, -1),
                new Vector3(5.5f, 0, -1),
                new Vector3(-5.5f, 0, -1),
                new Vector3(6.5f, 0, -1),
                new Vector3(-6.5f, 0, -1),
                new Vector3(7.5f, 0, -1),
                new Vector3(-7.5f, 0, -1),
                new Vector3(8.5f, 0, -1),
                new Vector3(-8.5f, 0, -1),
                new Vector3(9.5f, 0, -1),
                new Vector3(-9.5f, 0, -1),
                new Vector3(10.5f, 0, -1),
                new Vector3(-10.5f, 0, -1)
            },
            new List<Vector3>
            {
                new Vector3(0, 0, -2),
                new Vector3(1, 0, -2),
                new Vector3(-1, 0, -2),
                new Vector3(2, 0, -2),
                new Vector3(-2, 0, -2),
                new Vector3(3, 0, -2),
                new Vector3(-3, 0, -2),
                new Vector3(4, 0, -2),
                new Vector3(-4, 0, -2),
                new Vector3(5, 0, -2),
                new Vector3(-5, 0, -2),
                new Vector3(6, 0, -2),
                new Vector3(-6, 0, -2),
                new Vector3(7, 0, -2),
                new Vector3(-7, 0, -2),
                new Vector3(8, 0, -2),
                new Vector3(-8, 0, -2),
                new Vector3(9, 0, -2),
                new Vector3(-9, 0, -2),
                new Vector3(10, 0, -2),
                new Vector3(-10, 0, -2),
                new Vector3(11, 0, -2),
                new Vector3(-11, 0, -2)
            }
        };
        
        public static List<List<Vector3>> shipPositionVectors3D = new List<List<Vector3>>
        {
            new List<Vector3>
            {
                // Frontline vectors
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
                new Vector3(1, 1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, -1, 0),
                new Vector3(2, 0, 0),
                new Vector3(-2, 0, 0),
                new Vector3(0, 2, 0),
                new Vector3(0, -2, 0),
                new Vector3(2, 1, 0),
                new Vector3(-2, 1, 0),
                new Vector3(2, -1, 0),
                new Vector3(-2, -1, 0),
                new Vector3(1, 2, 0),
                new Vector3(-1, 2, 0),
                new Vector3(1, -2, 0),
                new Vector3(-1, -2, 0)
            },
            new List<Vector3>
            {
                new Vector3(.5f, 0, -1),
                new Vector3(-.5f, 0, -1),
                new Vector3(0, .5f, -1),
                new Vector3(0, -.5f, -1),
                new Vector3(1, .5f, -1),
                new Vector3(-1, .5f, -1),
                new Vector3(1, -.5f, -1),
                new Vector3(-1, -.5f, -1),
                new Vector3(0, 1.5f, -1),
                new Vector3(0, -1.5f, -1),
                new Vector3(1.5f, 0, -1),
                new Vector3(-1.5f, 0, -1),
                new Vector3(1.5f, 1.5f, -1),
                new Vector3(-1.5f, 1.5f, -1),
                new Vector3(1.5f, -1.5f, -1),
                new Vector3(-1.5f, -1.5f, -1),
            },
            new List<Vector3>
            {
                new Vector3(0, 0, -2),
                new Vector3(1, 0, -2),
                new Vector3(-1, 0, -2),
                new Vector3(0, 1, -2),
                new Vector3(0, -1, -2),
                new Vector3(1, 1, -2),
                new Vector3(-1, 1, -2),
                new Vector3(1, -1, -2),
                new Vector3(-1, -1, -2),
                new Vector3(2, 0, -2),
                new Vector3(-2, 0, -2),
                new Vector3(0, 2, -2),
                new Vector3(0, -2, -2),
                new Vector3(2, 1, -2),
                new Vector3(-2, 1, -2),
                new Vector3(2, -1, -2),
                new Vector3(-2, -1, -2),
                new Vector3(1, 2, -2),
                new Vector3(-1, 2, -2),
                new Vector3(1, -2, -2),
                new Vector3(-1, -2, -2)
            }
        };
    }
}