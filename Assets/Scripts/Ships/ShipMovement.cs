using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnitySteer.Behaviors;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipMovement : NetworkBehaviour
    {
        // Handles movement for all network entities, including ships

        [SyncVar(hook = nameof(HandleControlChanged))]
        public bool
            controlEnabled = false; // Control enabled -- can this entity be controlled by commanders on the team?

        [FormerlySerializedAs("entity")] public Ship ship; // Reference to the owning entity

        [SerializeField] private List<Steering> steeringBehaviors = new List<Steering>();

        [SerializeField] SteerForPursuit steerForPursuit;
        [SerializeField] SteerForPoint steerForPoint;
        [SerializeField] AutonomousVehicle autonomousVehicle;

        [SerializeField] private Renderer[] engines;
        
        public delegate void MoveToPointEvent(Vector3 pos, Quaternion orientation);

        public delegate void MovementEvent();

        public MoveToPointEvent OnMoveToPoint; // Delegate called when ship starts to move
        public MovementEvent OnArrival; // Delegate called when ship arrives or stops at destination

        // Reusable variables
        Ship tempShip;
        Transform transformToMoveTo; // for pursuit

        void HandleControlChanged(bool newControlEnabledState)
        {
            foreach (Steering behavior in steeringBehaviors)
            {
                behavior.enabled = newControlEnabledState;
            }
        }

        void Awake()
        {
            ship = GetComponent<Ship>();
            steerForPoint.OnArrival += HandleArrival;
            HandleControlChanged(controlEnabled); // Disable steering behaviors by default


            OnArrival += ShutOffEngines;
            OnMoveToPoint += TurnOnEngines;
            
            ShutOffEngines();
        }

        void ShutOffEngines()
        {
            StopCoroutine(nameof(FadeEngine)); // If the coroutine is already running, stop it so we don't have weird competing fades
            FadeEngine(false);
        }

        void TurnOnEngines(Vector3 pos, Quaternion orientation)
        {
            // Don't worry about the pos and orientation, we just need the for delegate subscription
            StopCoroutine(nameof(FadeEngine)); // If the coroutine is already running, stop it so we don't have weird competing fades
            FadeEngine(true);
        }

        // Coroutine variables
        private float currentEngineValue = 0; // Hold onto the current engine variable, which we will multiply by our engine color
        [SerializeField] float fadeTime = .5f; // how long does the fade occur?
        [SerializeField] private Color engineColor = Color.cyan;
        
        
        // Coroutines use an iterative type called IEnumerator
        IEnumerable FadeEngine(bool on) // We're going to fade between 0 and 1. Since 0 is black, we're going to multiply our main color by it
        {
            // Fade algorithm with coroutine 101
            
            float temporaryEngineValue = currentEngineValue; // Set a color value variable to our currently stored variable
            float timer = 0; // set up a timer to count up -- this variable needs to be 0 at coroutine start
            do // use a do instead of while to overload it past the > to force a clean 1.0 / 0.0 for the last frame of the fade
            {
                timer += Time.deltaTime; // IMPORTANT - add up the time since the last frame
                // We are going to use the ? operator (ternary) to determine what we fade to
                temporaryEngineValue = Mathf.Lerp(on ? currentEngineValue : 1, on ? currentEngineValue : 0, Mathf.Clamp(0, 1, timer / fadeTime)); // Fade the val 0-1 or 1-0, note that timer / fadeTime lerps 0-1 always
                foreach (Renderer ren in engines) // For each renderer in engines
                {
                    ren.material.SetColor("_BaseColor",  engineColor * temporaryEngineValue); // Lerps from black to cyan, since multiplying by 0 results in Color(0,0,0,0);
                }
                yield return null; // IMPORTANT - returns the thread so Unity can process the next frame -- i.e. pauses the coroutine here until next frame
            }
            while (timer <= fadeTime); // while timer is less than the fade time... obviously :P

            currentEngineValue = on ? 1 : 0; // Store the variable for future interpolation
        }
        
        
        

        // Callback for SteerForPoint's OnArrival, so subscribers don't need to connect to steering components directly
        private void HandleArrival(Steering obj)
        {
            OnArrival?.Invoke();
        }

        [Server]
        public void ServerPursue(uint entityId)
        {
            if (!controlEnabled) return;
            if(isServerOnly) // Player needs to be added to server, who won't receive RPC callback
                Pursue(ShipManager.GetEntityById(entityId));
            RpcPursue(entityId);
        }

        [ClientRpc]
        void RpcPursue(uint entityId)
        {
            Pursue(ShipManager.GetEntityById(entityId));
        }

        void Pursue(Ship ship)
        {
            ship.weaponSystemController.ClearTargets();
            steerForPoint.enabled = false;
            steerForPursuit.Quarry = ship.GetComponent<ShipMovement>().autonomousVehicle;
            steerForPursuit.enabled = true;
        }

        [Server]
        public void ServerMoveToPoint(Vector3 pos, Quaternion rot)
        {
            if (!controlEnabled)
            {
                return;
            }

            if(isServerOnly) // Player needs to be added to server, who won't receive RPC callback
                MoveToPoint(pos, rot);
            
            RpcMoveToPoint(pos, rot);
        }

        [ClientRpc]
        void RpcMoveToPoint(Vector3 pos, Quaternion rot) => MoveToPoint(pos, rot);

        void MoveToPoint(Vector3 pos, Quaternion rot)
        {
            steerForPoint.enabled = true;
            steerForPoint.TargetPoint = pos;
            steerForPursuit.enabled = false;
            OnMoveToPoint?.Invoke(pos, rot); 
        }

        [Server]
        public void ServerStopMovement()
        {
            if(isServerOnly) // Player needs to be added to server, who won't receive RPC callback
                StopMovement();
            RpcStopMovement();
        }

        [ClientRpc]
        void RpcStopMovement() => StopMovement();

        void StopMovement()
        {
            ship.weaponSystemController.ClearTargets();
            steerForPoint.TargetPoint = transform.position;
            steerForPoint.enabled = true;
            steerForPursuit.Quarry = null;
            steerForPursuit.enabled = false;
            OnArrival?.Invoke();
        }
        
        public void EnableMovement()
        {
            controlEnabled = true;
        }
        
        public void DisableMovement()
        {
            controlEnabled = false;
        }
    }
}