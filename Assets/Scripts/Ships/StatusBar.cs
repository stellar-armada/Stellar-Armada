using System.Collections;
using StellarArmada.Scenarios;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class StatusBar : MonoBehaviour
    {

        [SerializeField] Ship ship;
        private Hull hull;
        private Shield shield;

        [SerializeField] private Renderer statusBarRenderer;

        public string hullProperty = "_Health";
        public string shieldProperty = "_Shield";
        
        private MaterialPropertyBlock m;

        void Awake()
        {
            if(m == null) m = new MaterialPropertyBlock();

            SetVisibility(0);
            
             hull = ship.hull;
             shield = ship.shield;
             shield.ShieldChanged += SetShieldSlider;
             hull.HullChanged += SetHullSlider;
            
            LookAtMainCamera();
        }

        void SetVisibility(float v)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat("_Visibility", v);
            statusBarRenderer.SetPropertyBlock(m);
        }

        public void SetInsignia(Texture t)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetTexture("_Insignia", t);
            statusBarRenderer.SetPropertyBlock(m);
        }

        void SetHullSlider(float currentHull)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(hullProperty, currentHull / hull.maxHull);
            statusBarRenderer.SetPropertyBlock(m);
            
        }

        void SetShieldSlider(float currentShield)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(shieldProperty, currentShield / shield.maxShield);
            statusBarRenderer.SetPropertyBlock(m);
        }
        
        void LateUpdate ()
        {
            LookAtMainCamera();
        }

        void LookAtMainCamera()
        {
            if (PlayerCamera.instance == null) return;
            Camera c = PlayerCamera.instance.GetCamera();
            // Multiply the inverse rotation of our scene by the vector to face
            transform.rotation = c.transform.rotation;
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
            float currentVisibility = m.GetFloat("_Visibility");

            do
            {
                timer += Time.deltaTime;
                SetVisibility(Mathf.Lerp(currentVisibility, to, timer / fadeTime));
                yield return null;
            } while (timer < fadeTime);

            if (to.Equals(0))
            {
                statusBarRenderer.enabled = false;
            }
        }

        public void FadeOutStatusBar()
        {
            StartCoroutine(FadeVisibility(0));
        }
        
        // Set flag / icon for ship team identification
    }
}
