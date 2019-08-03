using UnityEngine;

namespace SpaceCommander.Player.Mobile
{
    public class CenterAttachmentPoint : MonoBehaviour
    {
        public static CenterAttachmentPoint instance;

        public Transform weaponAttachPoint;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }
    }
}
