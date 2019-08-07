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
            private Ship _ship;

            void Awake()
            {
                _ship = GetComponent<Ship>();
            }

            public void InitWarp() {
                _ship.visualModel.transform.localPosition = warpInStartPos;
                
                PrepareForWarpIn();
                
                StartCoroutine(WarpIn());
                
                Invoke(nameof(PrepareForControl), warpTime);
            }
            
            void PrepareForWarpIn()
            {
                _ship.shipMovement.controlEnabled = false;
                _ship.ShowHologram();
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

            void PrepareForControl()
            {
                _ship.shipMovement.controlEnabled = true;
                _ship.HideHologram();
            }
        }
   

}
