using UnityEngine;

namespace SpaceCommander.Ships
{
    public class StatusBar : MonoBehaviour
    {

        [SerializeField] Ship ship;
        private Transform cameraTransform;
        private ShipHull hull;
        private ShipShield shield;

        [SerializeField] private Renderer statusBarRenderer;

        public string hullProperty = "_Health";
        public string shieldProperty = "_Shield";

        public Texture insignia; // delete me

        private MaterialPropertyBlock m;

        void Awake()
        {
            m = new MaterialPropertyBlock();
             
             hull = ship.shipHull;
             shield = ship.shipShield;
             shield.ShieldChanged.AddListener(SetShieldSlider);
             hull.HullChanged.AddListener(SetHullSlider);
             cameraTransform = Camera.main.transform;
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
            LookAtMainCamera();
        }

        void LookAtMainCamera()
        {
            // TO-DO: Fix so that always facing camera, but also oriented nice vetically, not rotated with ship
            Vector3 targetPostition = new Vector3( cameraTransform.position.x, 
                transform.position.y, 
                cameraTransform.position.z ) ;
            transform.LookAt( targetPostition ) ;
        }
        
        public void ShowStatusBar()
        {
            statusBarRenderer.enabled = true;
        }
        
        public void HideStatusBar()
        {
            statusBarRenderer.enabled = false;
        }
        
        // Set flag / icon for ship team identification
    }
}
