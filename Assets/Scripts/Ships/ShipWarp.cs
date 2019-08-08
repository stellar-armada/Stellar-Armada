using System.Collections;
using Mirror;
using UnityEngine;
namespace SpaceCommander.Ships
{
    public class ShipWarp : NetworkBehaviour
        {
            // TO-DO: Fix warp-in and effect
            
            public Vector3 warpInStartPos;
            public float warpTime = 5f;
            private Ship ship;

            public bool isWarpedIn = false;
            
            void Awake()
            {
                ship = GetComponent<Ship>();
                PrepareForWarpIn();
                
                
                
                // for debugging
                Vector3 position = Random.onUnitSphere * 20f;
                Quaternion lookToCenter = Quaternion.LookRotation(-position);
                Debug.Log("position: " + position);
                
                InitWarp(position,lookToCenter);
            }
            
            void PrepareForWarpIn()
            {
                ship.shipMovement.controlEnabled = false;
                ship.weaponSystemController.weaponSystemsEnabled = false;
                ship.weaponSystemController.HideWeaponSystems();
                ship.statusBar.HideStatusBar();
                ship.shipShield.gameObject.SetActive(false);
                ship.visualModel.enabled = false;
                ship.ShowHologram();
            }
            
            public void InitWarp(Vector3 position, Quaternion rotation)
            {
                ship.transform.localPosition = position;
                ship.transform.rotation = rotation;
                ship.visualModel.transform.localPosition = warpInStartPos;
                ship.weaponSystemController.ShowWeaponSystems();
                ship.visualModel.enabled = true;
                
                StartCoroutine(WarpIn());
                
                Invoke(nameof(CompleteWarp), warpTime);
            }
            
            IEnumerator WarpIn()
            {
                float timer = 0f;
                do
                {
                    timer += Time.deltaTime;
                    ship.visualModel.transform.localPosition = Vector3.Lerp(warpInStartPos, Vector3.zero, timer / warpTime);
                    yield return null;
                }
                while (timer <= warpTime) ;
            }

            void CompleteWarp()
            {
                ship.shipMovement.controlEnabled = true;
                ship.weaponSystemController.weaponSystemsEnabled = true;
                ship.statusBar.ShowStatusBar();
                ship.shipShield.gameObject.SetActive(true);
                ship.shipShield.shieldEffectController.SetShieldActive(true, true);
                ship.HideHologram();
                isWarpedIn = true;
            }

         


        }
   

}
