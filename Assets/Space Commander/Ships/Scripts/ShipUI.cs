using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Ships
{
    public class ShipUI : MonoBehaviour
    {
        [SerializeField] Image frame;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] Slider hullSlider;
        [SerializeField] Slider shieldSlider;
        
        private Ship ship;
        private Transform cameraTransform;
        private ShipHealth health;
        void Awake()
        {
            ship = GetComponent<Ship>();
             health = ship.shipHealth;
             health.ShieldChanged.AddListener(SetShieldSlider);
             health.HullChanged.AddListener(SetHullSlider);
            
            cameraTransform = Camera.main.transform;
            LookAtMainCamera();
        }

        void SetHullSlider()
        {
            hullSlider.value = health.hull / health.maxHull;
            
        }

        void SetShieldSlider()
        {
            shieldSlider.value = health.shield / health.maxShield;
        }
        
        void LateUpdate ()
        {
            LookAtMainCamera();
        }

        void LookAtMainCamera()
        {
            // TO-DO: Fix so that always facing camera, but also oriented nice vetically, not rotated with ship
            Vector3 targetPostition = new Vector3( cameraTransform.position.x, 
                uiRoot.position.y, 
                cameraTransform.position.z ) ;
            uiRoot.LookAt( targetPostition ) ;
            
            
            
        }
        
        public void ShowUI()
        {
            uiCanvasGroup.alpha = 1f;
        }
        
        public void HideUI()
        {
            uiCanvasGroup.alpha = 0f;
        }
        
        // Show frame
        public void ShowFrame()
        {
            frame.enabled = true;
        }
        
        // Hide frame
        public void HideFrame()
        {
            frame.enabled = false;
        }

        // Set frame color

        public void SetFrameColor(Color color)
        {
            frame.color = color;
        }
        
        // Set flag / icon for ship team identification
    }
}
