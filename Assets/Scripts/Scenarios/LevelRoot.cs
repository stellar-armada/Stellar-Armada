using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Scenarios
{
    public class LevelRoot : MonoBehaviour
    {
        public static LevelRoot instance;
        void Awake()
        {
                instance = this;
            transform.localScale = Vector3.one;
        }

    }
}
