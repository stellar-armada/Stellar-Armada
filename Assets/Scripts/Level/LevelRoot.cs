using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Levels
{
    // Singleton object that contains everything in our world -- ships, the environment, our players (who are in ships), etc.
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