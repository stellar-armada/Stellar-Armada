using System.Collections;
using StellarArmada.Player;
using TMPro;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Class object that floats above ships in the minimap
    public class ShipStatusBar : MonoBehaviour
    {
        [SerializeField] Ship ship;
        private ShipHull _shipHull;
        private ShipShield _shipShield;

        [SerializeField] private TextMeshPro captainNameText;
        
        [SerializeField] private Renderer statusBarRenderer;

        public string hullProperty = "_Health";
        public string shieldProperty = "_Shield";
        
        void Awake()
        {
            captainNameText.renderer.enabled = false;
            ship.OnCaptainUpdated += HandleUpdateToCaptain;
            ship.OnEntityDead += HandleEntityDead;
            SetVisibility(0);
            
             _shipHull = ship.hull;
             _shipShield = ship.shield;
             _shipShield.ShieldChanged += SetShieldSlider;
             _shipHull.HullChanged += SetHullSlider;
            
            LookAtMainCamera();
            
            
        }

        void HandleEntityDead()
        {
            captainNameText.renderer.enabled = false;
            StartCoroutine(FadeVisibility(0));
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
            statusBarRenderer.material.SetFloat("_Visibility", v);
        }

        public void SetInsignia(Texture t)
        {
            statusBarRenderer.material.SetTexture("_Insignia", t);
        }

        void SetHullSlider(float currentHull)
        {
            statusBarRenderer.material.SetFloat(hullProperty, currentHull / _shipHull.maxHull);
        }

        void SetShieldSlider(float currentShield)
        {
            statusBarRenderer.material.SetFloat(shieldProperty, currentShield / _shipShield.maxShield);
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
                captainNameText.renderer.enabled = true;
            }
            float timer = 0;
            float fadeTime = .6f;
            float currentVisibility = statusBarRenderer.material.GetFloat("_Visibility");

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
