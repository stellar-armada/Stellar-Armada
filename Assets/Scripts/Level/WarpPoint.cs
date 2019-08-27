using UnityEngine;

namespace StellarArmada.Levels
{
// A placement marker in the level where battlegroups will warp in for various teams
// All levels need warp points. These function like "spawn points" in traditional FPS
    public class WarpPoint : MonoBehaviour
    {
        public uint teamIndex;
        public uint groupNumber;

        void Awake()
        {
            GetComponent<Renderer>().enabled = false;
        }

    }
}