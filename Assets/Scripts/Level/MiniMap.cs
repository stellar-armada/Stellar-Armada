using System.Collections;
using UnityEngine;
using UnitySteer.Behaviors;
using Zinnia.Extension;

namespace StellarArmada.Levels
{
    public class MiniMap : MonoBehaviour
    {
        public static MiniMap instance;
        
        // private GameObject miniMapSecene;
        public float yOffset = 1.2f;

        public float startScale = .04f;
        public float minScale;
        public float maxScale;

        public float rotationDamping = 10;

        [SerializeField] private LayerMask uiLayerMask;

        public bool interactable = false;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // miniMapSecene = Instantiate(scene);
            //  SetLayerRecursively(miniMapSecene, LayerUtil.LayerMaskToLayer(uiLayerMask));
            LocalPlayerBridgeSceneRoot.SceneRootCreated += InitializeMiniMap;
        }

        private bool lockRotation = true;

        public void ToggleRotationLock()
        {
            lockRotation = !lockRotation;
        }

        public void ShowMiniMap()
        {
            StartCoroutine(ExpandMiniMap());
        }
        
        IEnumerator ExpandMiniMap()
        {
            float timer = 0f;

            float expansionTime = .5f;

            do
            {
                timer += Time.deltaTime;
                transform.localScale = Vector3.one * Mathf.Lerp(0, startScale, timer / expansionTime);
                yield return null;
            } while (timer <= expansionTime);

            transform.localScale = Vector3.one * startScale;
            
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

            // TO-DO: Simplify references to create less garbage

            // 

            // miniMapSecene = Instantiate(scene);
            //  miniMapSecene.transform.SetParent(MiniMap.instance.transform);
            //  miniMapSecene.transform.localScale = Vector3.one;
            //  miniMapSecene.transform.localRotation = Quaternion.identity;

            // Put the minimap scene on on our UI ship layer for collision handling
            //  SetLayerRecursively(miniMapSecene, LayerUtil.LayerMaskToLayer(uiLayerMask));

            // Parent the MapTransformRoot to the SceneRoot (bride)
            MiniMapTransformRoot.instance.transform.SetParent(LocalPlayerBridgeSceneRoot.instance.transform, true);
            MiniMapTransformRoot.instance.transform.localPosition = new Vector3(0, yOffset, 0);
            MiniMapTransformRoot.instance.transform.SetGlobalScale(startScale * Vector3.one);

            // Parent the minimap to our MapTransformRoot object
            transform.SetParent(MiniMapTransformRoot.instance.transform);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }


        void LateUpdate()
        {
            if (!interactable || !lockRotation || MiniMapController.instance == null || MiniMapController.instance.isActive) return;
            transform.rotation =
                Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationDamping);

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