using UnityEngine;

namespace StellarArmada.Levels {
    public class MiniMapTransformRoot : MonoBehaviour
    {
        // Instead of manipulating the MiniMap directly
        // The MiniMapController parents the minimap to this object after placing this object between the controllers
        // So the transformation happens relative to othe local player

        public static MiniMapTransformRoot instance; // singleton accessor

        void Awake()
        {
            instance = this;
        }
    }
}
