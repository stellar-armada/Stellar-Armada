using UnityEngine;
namespace SpaceCommander
{
    public class ScaleManager : MonoBehaviour
    {
        static float scale = .0004f; // magic number constant
        public delegate void ScaleEventHandler(float newScale);

        public static ScaleEventHandler OnScaleEventHandler;

        void Awake()
        {
            OnScaleEventHandler = null; // clear delegates
        }

        public static void SetScale(float newScale)
        {
            scale = newScale;
            OnScaleEventHandler?.Invoke(newScale);
        }

        public static float GetScale() => scale;
    }
}