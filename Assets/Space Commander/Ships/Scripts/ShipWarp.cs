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
            [SerializeField] bool warpInOnStart = true;

            void Awake()
            {
                ship = GetComponent<Ship>();
            }
            void Start()
            {
                if (warpInOnStart) InitWarp();
            }

            public void InitWarp() {
                ship.visualModel.transform.localPosition = warpInStartPos;
                
                PrepareForWarpIn();
                
                StartCoroutine(WarpIn());
                
                Invoke(nameof(PrepareForControl), warpTime);
            }
            
            void PrepareForWarpIn()
            {
                ship.shipMovement.controlEnabled = false;
                ship.ShowHologram();
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

            void PrepareForControl()
            {
                ship.shipMovement.controlEnabled = true;
                ship.HideHologram();
            }
        }
   

}
