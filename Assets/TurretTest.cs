using UnityEngine;

namespace SpaceCommander.Weapons.Tests
{
    public class TurretTest : MonoBehaviour
    {
        public static TurretTest instance;
        RaycastHit hitInfo; // Raycast structure
        public Turret turret;
        bool isFiring; // Is turret currently in firing state

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            CheckForTurn();
            CheckForFire();
        }

        void CheckForFire()
        {
            // Fire turret
            if (!isFiring && Input.GetKeyDown(KeyCode.Mouse0))
            {
                isFiring = true;
                turret.Fire();
            }

            // Stop firing
            if (isFiring && Input.GetKeyUp(KeyCode.Mouse0))
            {
                isFiring = false;
                turret.Stop();
            }
        }
        
        

        void CheckForTurn()
        {
            // Construct a ray pointing from screen mouse position into world space
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast
            if (Physics.Raycast(cameraRay, out hitInfo, 500f))
            {
                turret.SetNewTarget(hitInfo.point);
            }
        }
    }
}