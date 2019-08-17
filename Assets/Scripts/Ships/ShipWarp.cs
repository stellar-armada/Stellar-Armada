using System.Collections;
using Mirror;
using UnityEngine;
#pragma warning disable 0649
namespace SpaceCommander.Ships
{
    public class ShipWarp : NetworkBehaviour
        {
            // TO-DO: Fix warp-in and effect
            
            public Vector3 warpInStartPos;
            public float warpTime = 5f;
            private Ship _ship;

            public bool isWarpedIn = false;
            
            void Awake()
            {
                _ship = GetComponent<Ship>();
                PrepareForWarpIn();
                
                
                
                // for debugging
                Vector3 position = Random.onUnitSphere * 20f;
                Quaternion lookToCenter = Quaternion.LookRotation(-position);
                
                InitWarp(position,lookToCenter);
            }
            
            void Start()
            {
                transform.localPosition = Random.onUnitSphere * 10f;
                // For debug
            }
            
            void PrepareForWarpIn()
            {
                _ship.movement.DisableMovement();
                _ship.weaponSystemController.weaponSystemsEnabled = false;
                _ship.weaponSystemController.HideWeaponSystems();
                _ship.statusBar.HideStatusBar();
                _ship.shield.gameObject.SetActive(false);
                _ship.visualModel.enabled = false;
                //ship.ShowHologram();
            }
            
            public void InitWarp(Vector3 position, Quaternion rotation)
            {
                _ship.transform.localPosition = position;
                _ship.transform.rotation = rotation;
                _ship.visualModel.transform.localPosition = warpInStartPos;
                _ship.weaponSystemController.ShowWeaponSystems();
                _ship.visualModel.enabled = true;
                
                StartCoroutine(WarpIn());
                
                Invoke(nameof(CompleteWarp), warpTime);
            }
            
            IEnumerator WarpIn()
            {
                float timer = 0f;
                do
                {
                    timer += Time.deltaTime;
                    _ship.visualModel.transform.localPosition = Vector3.Lerp(warpInStartPos, Vector3.zero, timer / warpTime);
                    yield return null;
                }
                while (timer <= warpTime) ;
            }

            void CompleteWarp()
            {
                _ship.movement.EnableMovement();
                _ship.weaponSystemController.weaponSystemsEnabled = true;
                _ship.statusBar.ShowStatusBar();
                _ship.shield.gameObject.SetActive(true);
                _ship.shield.shieldEffectController.SetShieldActive(true, true);
                //ship.HideHologram();
                isWarpedIn = true;
            }

         


        }
   

}
