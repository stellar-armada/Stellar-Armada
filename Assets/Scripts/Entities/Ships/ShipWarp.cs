using System.Collections;
using Mirror;
using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipWarp : NetworkBehaviour
        {
            // TO-DO: Fix warp-in and effect
            public GameObject warpFx;
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
                warpFx.SetActive(false);
            }
            
            public void InitWarp(Vector3 position, Quaternion rotation)
            {
                ship.transform.SetParent(LevelRoot.instance.transform); // if not done already. check to see if it's already called by now
                ship.transform.localPosition = position;
                ship.transform.localRotation = rotation;
                ship.visualModel.transform.localPosition = warpInStartPos;
                ship.weaponSystemController.ShowWeaponSystems();
                ship.visualModel.enabled = true;
                Debug.Log("<color=orange>WARP</color> InitWarp()");
                StartCoroutine(WarpIn());
                
                Invoke(nameof(CompleteWarp), warpTime);
                
                warpFx.SetActive(true);
            }
            
            IEnumerator WarpIn()
            {
                Debug.Log("<color=orange>WARP</color> WarpIn()");
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
                Debug.Log("<color=orange>WARP</color> CompleteWarp()");
                ship.movement.CmdEnableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = true;
                ship.miniMapStatusBar.ShowStatusBar();
                ship.shield.gameObject.SetActive(true);
                ship.shield.shieldEffectController.SetShieldActive(true, true);
                isWarpedIn = true;
                
                // Disable warpFX
                
                var main = warpFx.GetComponent<ParticleSystem>().main;
                main.loop = false;
                
                foreach (ParticleSystem system in warpFx.GetComponentsInChildren<ParticleSystem>())
                {
                    main = system.main;
                    main.loop = false;
                }

                if (ship.captain == PlayerController.localPlayer)
                {
                    Debug.Log("Move this to a delegate");
                    if(PlatformManager.instance.Platform == PlatformManager.PlatformType.VR)
                    VRMiniMap.instance.ShowMiniMap();
                }
            }

         


        }
   

}
