using UnityEngine;
using System.Collections;
using InputHandling;
#if  PLATFORM_LUMIN
using MagicLeap.UI;
#endif

namespace SpaceCommander.UI
{
    public class Pointer : MonoBehaviour
    {
        public static Pointer instance; // singleton accessor
        private MaterialPropertyBlock props;
        #region Private Variables Serialized In Inspector 
        [SerializeField] Transform uiPointerTip;
        [SerializeField] public Transform pointerStartPoint;
        [SerializeField] LayerMask uiLayer;
        [SerializeField] float speed = 2.0f;
        [SerializeField] float glowIntensity;
        [SerializeField] Renderer menuButtonRenderer;
        [SerializeField] Color buttonGlowColor;
        #endregion

        #region Private Variables
        LineRenderer lineRenderer;
        GameObject go;
        bool isPulsing = false;
#if  PLATFORM_LUMIN
        MLInputModule inputModule;
#endif
        #endregion

        #region Initialization and Deinitialization
        void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Pointer already exists. Destroying.");
                Destroy(instance);
            }
            instance = this;
               props = new MaterialPropertyBlock();
            lineRenderer = GetComponent<LineRenderer>();
            StartMenuButtonGlow();
        }

        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region Public Methods

        public void ShowPointer()
        {
            gameObject.SetActive(true);
        }

        public void HidePointer()
        {
            gameObject.SetActive(false);
        }

        public void StartMenuButtonGlow()
        {
            StartCoroutine(PulseGlowMenuButton());
        }
        
        public void StopMenuButtonGlow()
        {
            isPulsing = false;
        }
        
        #endregion

        #region Private Regions

        void Update()
        {
#if PLATFORM_LUMIN
                    UpdateMagicLeap();
#endif

        }
#if PLATFORM_LUMIN
        void UpdateMagicLeap()
        {
            if (MLInputModuleReference.instance == null) return;
            if (MLInputModuleReference.instance.inputModule.ControlsLineSegment[0].End.HasValue)
            {
             SetRaycastLine(true, MLInputModuleReference.instance.inputModule.ControlsLineSegment[0].End.Value);
            }
            else
            {
                             SetRaycastLine(false, pointerStartPoint.position);
            }
        }
#endif


        public void SetRaycastLine(bool active, Vector3 endPosition)
        {
            if (active)
            {
                lineRenderer.enabled = true;

                lineRenderer.SetPositions(new [] { pointerStartPoint.position, endPosition });

                uiPointerTip.position = endPosition;

                uiPointerTip.gameObject.SetActive(true);
            }
            else
            {
                lineRenderer.enabled = false;

                uiPointerTip.position = endPosition;

                uiPointerTip.gameObject.SetActive(false);
            }
        }

        IEnumerator PulseGlowMenuButton()
        {
            isPulsing = true;

            float val = 0;

            float timer = 0;

            Color eCol = buttonGlowColor;


            while (isPulsing)
            {
                timer += Time.deltaTime;

                val = Mathf.PingPong(timer * speed, 1.0f) * glowIntensity;

                props.SetColor("_EmissionColor", eCol * Mathf.LinearToGammaSpace(val));
                menuButtonRenderer.SetPropertyBlock(props);
                

                yield return null;
            }

            props.SetColor("_EmissionColor", eCol * Mathf.LinearToGammaSpace(val));
            menuButtonRenderer.SetPropertyBlock(props);
        }

        #endregion
    }
}