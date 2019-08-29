using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StellarArmada.Levels
{
    // The actual level object, generally the root of the "level" prefab
    // Level is selected by the scenario, which sets up some meta information and instantiates the level prefab
    public class Level : MonoBehaviour
    {
        public List<WarpPoint> warpPoints = new List<WarpPoint>();
        public static Level currentLevel;

        void Awake()
        {
            currentLevel = this;
        }
    }
}