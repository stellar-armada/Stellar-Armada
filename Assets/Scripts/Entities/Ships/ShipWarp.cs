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
            
            [Command]
            public void CmdInitWarp(Vector3 position, Quaternion rotation)
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
                ship.movement.EnableMovement();
                ship.weaponSystemController.weaponSystemsEnabled = true;
                ship.miniMapStatusBar.ShowStatusBar();
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

                if (ship.captain == HumanPlayerController.localPlayer)
                {
                    MiniMap.instance.ShowMiniMap();
                }
            }

         


        }
   

}
