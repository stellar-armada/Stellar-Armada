using Mirror;
using SpaceCommander.Common.Tests;
using UnityEngine;

namespace SpaceCommander.Weapons.Tests
{
    public class TurretTest : MonoBehaviour
    {
        public static TurretTest instance;
        RaycastHit hitInfo; // Raycast structure
        public Turret[] turrets;
        bool isFiring; // Is turret currently in firing state
        
        [SerializeField] GameObject sharedNetworkManagers;

        private bool hasBeenSet = false;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            NetworkManager.singleton.StartHost();
            GameObject sharedNetworkManagerObject = Instantiate(sharedNetworkManagers);
            NetworkServer.Spawn(sharedNetworkManagerObject);

        }
        void Update()
        {
            if (!hasBeenSet)
            {
                foreach (Turret turret in turrets)
            {
                if (turret.GetWeaponSystemController().GetEntity().GetPlayer() == null)
                {
                    Debug.Log("Owning player is null");
                    TestPlayer player = FindObjectOfType<TestPlayer>();
                    if (player != null)
                    {
                        turret.GetWeaponSystemController().GetEntity().CmdSetPlayer(player.netId);
                    }
                }
                else
                {
                    hasBeenSet = true;
                }
            }
            }

            CheckForTurn();
            CheckForFire();
        }

        void CheckForFire()
        {
            // Fire turret
            if (!isFiring && Input.GetKeyDown(KeyCode.Mouse0))
            {
                isFiring = true;
                foreach (Turret turret in turrets)
                {
                    turret.StartFiring();
                }
            }

            // Stop firing
            if (isFiring && Input.GetKeyUp(KeyCode.Mouse0))
            {
                isFiring = false;
                foreach (Turret turret in turrets)
                {
                    turret.StopFiring();
                }
            }
        }
        
        

        void CheckForTurn()
        {
            // Construct a ray pointing from screen mouse position into world space
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast
            if (Physics.Raycast(cameraRay, out hitInfo, 500f))
            {
                foreach (Turret turret in turrets)
                {
                    turret.SetNewTarget(hitInfo.point);
                }
            }
        }
    }
}