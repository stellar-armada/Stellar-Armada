using System.Collections;
using Mirror;
using StellarArmada.Levels;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
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

            }

            void PrepareForWarpIn()
            {
                ship.movement.CmdDisableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = false;
                ship.weaponSystemController.HideWeaponSystems();
                ship.shield.gameObject.SetActive(false);
                ship.visualModel.enabled = false;
            }
            
            public void InitWarp(Vector3 position, Quaternion rotation)
            {
                ship.transform.SetParent(LevelRoot.instance.transform); // if not done already. check to see if it's already called by now
                ship.transform.localPosition = position;
                ship.transform.localRotation = rotation;
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
                ship.movement.CmdEnableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = true;
                ship.miniMapStatusBar.ShowStatusBar();
                ship.shield.gameObject.SetActive(true);
                ship.shield.shieldEffectController.SetShieldActive(true, true);
                isWarpedIn = true;
            }

         


        }
   

}
