using UnityEngine;

namespace StellarArmada
{
    public class LocalPlayerRig : MonoBehaviour
    {
        public static LocalPlayerRig instance;
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