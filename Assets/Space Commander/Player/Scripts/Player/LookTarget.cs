using UnityEngine;

namespace SpaceCommander.Player
{
    public class LookTarget : MonoBehaviour
    {

        [SerializeField] private float defaultZ = 20f; 

        [SerializeField] private Transform cam;
        
        public static LookTarget instance;

        private RaycastHit hit;

        [SerializeField] private LayerMask layerMask;

        private bool fixedDistance = true;

        private Transform t;
        
        void Awake()
        {
            instance = this;
            t = transform;
            if (fixedDistance)
            {
                t.localPosition = Vector3.forward * defaultZ;
            }

        }

        void FixedUpdate()
        {
            if (fixedDistance) return;
            if (Physics.Raycast(cam.position, cam.forward, out hit, layerMask))
            {
                t.position = hit.point;
            }
            else
            {
                t.localPosition = Vector3.forward * defaultZ;
            }
        }

    }
}
