using UnityEngine;


#pragma warning disable 0649
namespace Wacki {
    abstract public class IUIPointer : MonoBehaviour {

        public GameObject hitPoint;
        public GameObject pointer;
        
        [SerializeField] float laserThickness = .02f;

        private float _distanceLimit;

        [SerializeField] LayerMask layerMask;

        public delegate void UiPointerEvent(bool isPointingAtCanvas);

        public UiPointerEvent OnCanvasStateChanged;

        // Use this for initialization
        void Start()
        {

            // initialize concrete class
            Initialize();
            
            // register with the LaserPointerInputModule
            if(LaserPointerInputModule.instance == null) {
                new GameObject().AddComponent<LaserPointerInputModule>();
            }
            

            LaserPointerInputModule.instance.AddController(this);
        }

        void OnDestroy()
        {
            if(LaserPointerInputModule.instance != null)
                LaserPointerInputModule.instance.RemoveController(this);
        }

        protected virtual void Initialize()
        {
            hitPoint.SetActive(false);
            pointer.SetActive(false);
            
        }

        public virtual void OnEnterControl(GameObject control)
        {
            Debug.Log("OnEnterControl called");
            OnCanvasStateChanged?.Invoke(true);
            hitPoint.SetActive(true);
            pointer.SetActive(true);
        }

        public virtual void OnExitControl(GameObject control)
        {
            Debug.Log("OnExitControl called");
            OnCanvasStateChanged?.Invoke(false);
            hitPoint.SetActive(false);
            pointer.SetActive(false);
        }
        abstract public bool ButtonDown();
        abstract public bool ButtonUp();

        protected virtual void Update()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            bool bHit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);

            float distance = 100.0f;

            if(bHit) {
                distance = hitInfo.distance;
            }

            // ugly, but has to do for now
            if(_distanceLimit > 0.0f) {
                distance = Mathf.Min(distance, _distanceLimit);
                bHit = true;
            }

            pointer.transform.localScale = new Vector3(laserThickness, laserThickness, distance);

            if(bHit) {
                hitPoint.SetActive(true);

                hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }
            else {
                hitPoint.SetActive(false);

            }

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }

        // limits the laser distance for the current frame
        public virtual void LimitLaserDistance(float distance)
        {
            if(distance < 0.0f)
                return;

            if(_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }
    }

}