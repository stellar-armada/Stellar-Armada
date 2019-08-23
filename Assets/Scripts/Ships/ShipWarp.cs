using System.Collections;
using Mirror;
using StellarArmada.Scenarios;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Ships
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

            }

            void PrepareForWarpIn()
            {
                _ship.movement.CmdDisableMovement();
                _ship.weaponSystemController.weaponSystemsEnabled = false;
                _ship.weaponSystemController.HideWeaponSystems();
                _ship.shield.gameObject.SetActive(false);
                _ship.visualModel.enabled = false;
            }
            
            public void InitWarp(Vector3 position, Quaternion rotation)
            {
                _ship.transform.SetParent(LevelRoot.instance.transform); // if not done already. check to see if it's already called by now
                Debug.Log("Warping ship to: " + position);
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
                _ship.movement.CmdEnableMovement();
                _ship.weaponSystemController.weaponSystemsEnabled = true;
                _ship.statusBar.ShowStatusBar();
                _ship.shield.gameObject.SetActive(true);
                _ship.shield.shieldEffectController.SetShieldActive(true, true);
                isWarpedIn = true;
            }

         


        }
   

}
