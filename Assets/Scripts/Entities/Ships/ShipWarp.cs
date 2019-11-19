using System.Collections;
using Mirror;
using StellarArmada.Levels;
using StellarArmada.Match;
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
            
            void Awake()
            {
                ship = GetComponent<Ship>();
                PrepareForWarpIn();

            }

            void PrepareForWarpIn()
            {
                ship.movement.DisableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = false;
                ship.weaponSystemController.HideWeaponSystems();
                
                ship.shield.gameObject.SetActive(false);
                ship.visualModel.enabled = false;
                warpFx.SetActive(false);
            }
            
            [Server]
            public void ServerInitWarp(Vector3 position, Quaternion rotation)
            {
                
                if (isServerOnly) InitWarp(position, rotation);
                RpcInitWarp(position, rotation);
            }

            [ClientRpc]
            public void RpcInitWarp(Vector3 position, Quaternion rotation)
            {
                InitWarp(position, rotation);
            }

            void InitWarp(Vector3 position, Quaternion rotation)
            {
                Transform s = ship.transform;
                s.SetParent(LevelRoot.instance.transform); // if not done already. check to see if it's already called by now
                s.localPosition = position;
                s.localRotation = rotation;
                ship.visualModel.transform.localPosition = warpInStartPos;
                ship.weaponSystemController.ShowWeaponSystems();
                ship.visualModel.enabled = true;
                
                warpFx.SetActive(true);

                MatchStateManager.instance.EventOnMatchStart += WarpIn;
            }

            void WarpIn()
            {
                
                StartCoroutine(IWarpIn());
                
                Invoke(nameof(CompleteWarp), warpTime);
            }
            
            IEnumerator IWarpIn()
            {
                float timer = 0f;
                do
                {
                    timer += Time.deltaTime;
                    ship.visualModel.transform.localPosition = Vector3.Lerp(warpInStartPos, Vector3.zero, timer / warpTime);
                    yield return null;
                }
                while (timer <= warpTime) ;

                ship.visualModel.transform.localPosition = Vector3.zero;

            }
            


            void CompleteWarp()
            {
                ship.movement.EnableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = true;
                ship.shipStatusBar.ShowStatusBar();
                ship.shield.gameObject.SetActive(true);
                ship.shield.shieldEffectController.SetShieldActive(true, true);
                
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
