using System.Collections;
using StellarArmada.Player;
using TMPro;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    // Class object that floats above ships in the minimap
    public class MiniMapStatusBar : MonoBehaviour
    {
        [SerializeField] Ship ship;
        private EntityHull entityHull;
        private EntityShield entityShield;

        [SerializeField] private TextMeshPro captainNameText;
        
        [SerializeField] private Renderer statusBarRenderer;

        public string hullProperty = "_Health";
        public string shieldProperty = "_Shield";
        
        private MaterialPropertyBlock m;

        void Awake()
        {
            captainNameText.renderer.enabled = false;
            ship.OnCaptainUpdated += HandleUpdateToCaptain;
            
            m = new MaterialPropertyBlock();

            SetVisibility(0);
            
             entityHull = ship.hull;
             entityShield = ship.shield;
             entityShield.ShieldChanged += SetShieldSlider;
             entityHull.HullChanged += SetHullSlider;
            
            LookAtMainCamera();
        }

        void Start()
        {
            if (ship.captain != null)
            {
                HandleUpdateToCaptain();
            }
        }

        void HandleUpdateToCaptain()
        {
            captainNameText.renderer.enabled = true;
            captainNameText.text = ship.captain.playerName;
            captainNameText.color = ship.GetTeam().color;
            ship.captain.EventOnPlayerNameChange += HandleNameChange;
        }

        void HandleNameChange()
        {
            captainNameText.text = ship.captain.playerName;
        }

        void SetVisibility(float v)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat("_Visibility", v);
            statusBarRenderer.SetPropertyBlock(m);
        }

        public void SetInsignia(Texture t)
        {
            if(m == null) m = new MaterialPropertyBlock();
            statusBarRenderer.GetPropertyBlock(m);
            m.SetTexture("_Insignia", t);
            statusBarRenderer.SetPropertyBlock(m);
        }

        void SetHullSlider(float currentHull)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(hullProperty, currentHull / entityHull.maxHull);
            statusBarRenderer.SetPropertyBlock(m);
            
        }

        void SetShieldSlider(float currentShield)
        {
            statusBarRenderer.GetPropertyBlock(m);
            m.SetFloat(shieldProperty, currentShield / entityShield.maxShield);
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
            
            // Linear algebra!
            
            // Compute the distance away from the plane (on the Y) our camera is (in local space relation to this status bar)
            float distanceToPlane = Vector3.Dot(transform.up, c.transform.position - transform.position);
            
            // Subtract the local Y so that we reduce our rotation to 2 dimensions (since they are both y = 0)
            Vector3 pointOnPlane = c.transform.position - (transform.up * distanceToPlane);
 
            // Look at the 2D version of the camera position vector
            transform.LookAt(pointOnPlane, transform.up);
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
