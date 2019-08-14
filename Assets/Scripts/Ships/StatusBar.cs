using System.Collections;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class StatusBar : MonoBehaviour
    {

        [SerializeField] Ship ship;
        private ShipHull hull;
        private ShipShield shield;

        [SerializeField] private Renderer statusBarRenderer;

        public string hullProperty = "_Health";
        public string shieldProperty = "_Shield";

        public Texture insignia; // delete me

        private MaterialPropertyBlock m;

        void Awake()
        {
            if(m == null) m = new MaterialPropertyBlock();
             
             hull = ship.shipHull;
             shield = ship.shipShield;
             shield.ShieldChanged.AddListener(SetShieldSlider);
             hull.HullChanged.AddListener(SetHullSlider);
             
             Debug.Log("Status bar needs local player reference");

            LookAtMainCamera();
        }

        public void SetInsignia(Texture t)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetTexture("_Insignia", t);
            statusBarRenderer.SetPropertyBlock(m);
        }

        void SetHullSlider()
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(hullProperty, hull.currentHull / hull.maxHull);
            statusBarRenderer.SetPropertyBlock(m);
            
        }

        void SetShieldSlider()
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(shieldProperty, shield.currentShield / shield.maxShield);
            statusBarRenderer.SetPropertyBlock(m);
        }
        
        void LateUpdate ()
        {
            Debug.Log("Skipping late update while camera transform gets fixed");
            return;
            LookAtMainCamera();
        }

        void LookAtMainCamera()
        {
            if (PlayerCamera.instance == null) return;
            Camera c = PlayerCamera.instance.GetCamera();
            Transform cameraTransform = c.transform;
            Vector3 targetPostition = new Vector3( cameraTransform.position.x, 
                transform.position.y, 
                cameraTransform.position.z ) ;
            transform.LookAt( targetPostition ) ;
        }
        
        public void ShowStatusBar()
        {
            StartCoroutine(FadeVisibility(1));
        }

        IEnumerator FadeVisibility(float to)
        {
            if (to > 0)
            {
                statusBarRenderer.enabled = true;
            }
            float timer = 0;
            float fadeTime = .6f;
            statusBarRenderer.GetPropertyBlock(m);
            float currentVisibility = m.GetFloat("_Visibility");

            do
            {
                timer += Time.deltaTime;
                m.SetFloat("_Visibility", Mathf.Lerp(currentVisibility, to, timer / fadeTime));
                yield return null;
            } while (timer < fadeTime);

            if (to.Equals(0))
            {
                statusBarRenderer.enabled = false;
            }
        }
        
        public void HideStatusBar()
        {
            if(m == null) m = new MaterialPropertyBlock();
            statusBarRenderer.enabled = false;
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat("_Visibility", 0);
        }
        
        public void FadeOutStatusBar()
        {
            StartCoroutine(FadeVisibility(0));
        }
        
        // Set flag / icon for ship team identification
    }
}
