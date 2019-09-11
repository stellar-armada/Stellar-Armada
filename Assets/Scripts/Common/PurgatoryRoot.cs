using UnityEngine;

namespace StellarArmada
{
    public class PurgatoryRoot : MonoBehaviour
    {
        public static PurgatoryRoot instance;
        void Awake()
        {
            instance = this;
        }
        
        public void Enable()
        {
        gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}