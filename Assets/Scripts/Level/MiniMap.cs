using System.Collections;
using StellarArmada.IO;
using UnityEngine;
using UnitySteer.Behaviors;
using Zinnia.Extension;

namespace StellarArmada.Levels
{
    public class MiniMap : MonoBehaviour
    {
        public static MiniMap instance;

        public float yOffset = 1.2f;

        public float startScale = .04f;
        public float minScale;
        public float maxScale;

        public float rotationDamping = 10f;

        //public float positionDamping = 10f;

        [SerializeField] private LayerMask uiLayerMask;

        public bool interactable = false;

        private Transform t;
        private Transform miniMapTransformRoot;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            t = transform;
            miniMapTransformRoot = MiniMapTransformRoot.instance.transform;

            InputManager.instance.OnLeftThumbstickButton += (down) =>
            {
                if (HandSwitcher.instance.CurrentHandIsLeft() && down) ToggleRotationLock();
            };
            
            InputManager.instance.OnRightThumbstickButton += (down) =>
            {
                if (HandSwitcher.instance.CurrentHandIsRight() && down) ToggleRotationLock();
            };

            // miniMapSecene = Instantiate(scene);
            //  SetLayerRecursively(miniMapSecene, LayerUtil.LayerMaskToLayer(uiLayerMask));
        }

        private bool lockRotation = false;

        public void ToggleRotationLock()
        {
            lockRotation = !lockRotation;
        }

        public void ShowMiniMap()
        {
            InitializeMiniMap();
            StartCoroutine(ExpandMiniMap());
        }

        IEnumerator ExpandMiniMap()
        {
            float timer = 0f;

            float expansionTime = .5f;

            do
            {
                timer += Time.deltaTime;
                transform.SetGlobalScale(Vector3.one * Mathf.Lerp(0, startScale, timer / expansionTime));
                yield return null;
            } while (timer <= expansionTime);

            transform.SetGlobalScale(Vector3.one * startScale);

            interactable = true;
        }

        public void LockRotation()
        {
            lockRotation = true;
        }

        public void UnlockRotation()
        {
            lockRotation = false;
        }

        void InitializeMiniMap()
        {
            // unparent minimap before transformation happens
            t.SetParent(LocalPlayerBridgeSceneRoot.instance.transform, true);

            // Parent the MapTransformRoot to the SceneRoot (bride)
            miniMapTransformRoot.SetParent(LocalPlayerBridgeSceneRoot.instance.transform, true);
            miniMapTransformRoot.localScale = Vector3.one;
            miniMapTransformRoot.localPosition = new Vector3(0, yOffset, 0);

            // Parent the minimap to our MapTransformRoot object
            t.SetParent(miniMapTransformRoot);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
        }


        void Update()
        {
            UpdateLockedOrientation();
        }

        void UpdateLockedOrientation()
        {
            if (!interactable || !lockRotation || MiniMapController.instance == null || MiniMapController.instance.isActive) return;

            // Make sure our transform root is properly set up
            if (t.parent != miniMapTransformRoot)
            {
                miniMapTransformRoot.position = t.position;
                miniMapTransformRoot.rotation = t.rotation;
                t.SetParent(miniMapTransformRoot);
            }

            // Lerp the minimap transform root toward the default 0
            //miniMapTransformRoot.localPosition = Vector3.Lerp(miniMapTransformRoot.localPosition, Vector3.up * yOffset, Time.deltaTime * positionDamping);
            miniMapTransformRoot.localRotation =  Quaternion.Slerp(miniMapTransformRoot.localRotation, Quaternion.identity, Time.deltaTime * rotationDamping);

            // Offset the minimap by the local transform of itself -- thus centering the minimap positionally in space
            //t.localPosition = Vector3.Lerp(Vector3.zero, -t.TransformPoint(t.position), Time.deltaTime * positionDamping);
            //t.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * rotationDamping);
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;

            obj.layer = newLayer;

            // Destroy any detectables, also, so we don't throw off our ship nav
            var detectable = obj.GetComponent<DetectableObject>();
            if (detectable) Destroy(detectable);

            foreach (Transform child in obj.transform)
            {
                if (child != null) SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}