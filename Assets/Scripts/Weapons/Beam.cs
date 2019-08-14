using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{ 
    [RequireComponent(typeof (LineRenderer))]
    public class Beam : Damager, ISpawnable
    {
        public Texture[] BeamFrames; // Animation frame sequence
        public float FrameStep; // Animation time

        public float beamScale; // Default beam scale to be kept over distance
        public float MaxBeamLength; // Maximum beam length

        public bool AnimateUV; // UV Animation
        public float UVTime; // UV Animation speed

        LineRenderer lineRenderer; // Line rendered component
        RaycastHit hitPoint; // Raycast structure
        RaycastHit2D hitPoint2D; // Raycasthit in 2d

        int frameNo; // Frame counter
        int FrameTimerID; // Frame timer reference
        float beamLength; // Current beam length
        float initialBeamOffset; // Initial UV offset 

        private bool hit;

        void Awake()
        {
            // Get line renderer component
            lineRenderer = GetComponent<LineRenderer>();

            // Assign first frame texture
            if (!AnimateUV && BeamFrames.Length > 0)
                lineRenderer.material.SetTexture("_BaseMap", BeamFrames[0]);

            // Randomize uv offset
            initialBeamOffset = Random.Range(0f, 5f);
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            hit = false;
            // Do one time raycast in case of one shot flag
                Raycast();

            // Start animation sequence if beam frames array has more than 2 elements
            if (BeamFrames.Length > 1)
                Animate();
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
            Debug.Log("Despawned");
            // Reset frame counter
            frameNo = 0;

            // Clear timer
            if (FrameTimerID != -1)
            {
                TimeManager.instance.RemoveTimer(FrameTimerID);
                FrameTimerID = -1;
            }
        }

        // Hit point calculation
        void Raycast()
        { 
            // Prepare structure and create ray
            hitPoint = new RaycastHit();
            Ray ray = new Ray(transform.position, transform.forward);
            // Calculate default beam proportion multiplier based on default scale and maximum length
            float propMult = MaxBeamLength*(beamScale/10f);

            // Raycast
            if (Physics.Raycast(ray, out hitPoint, MaxBeamLength, layerMask))
            {
                // Get current beam length and update line renderer accordingly
                beamLength = Vector3.Distance(transform.position, hitPoint.point);
                lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));

                // Calculate default beam proportion multiplier based on default scale and current length
                propMult = beamLength*(beamScale/10f);
                // Spawn prefabs and apply force
                owningWeaponSystem.Impact(hitPoint.point);
                hit = true;
                Debug.Log("Hit " + hitPoint.transform.name);
            }
            //checking in 2d mode
            else
            {
                RaycastHit2D ray2D = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
                    new Vector2(transform.forward.x, transform.forward.y), beamLength, layerMask);
                if (ray2D)
                {
                    // Get current beam length and update line renderer accordingly
                    beamLength = Vector3.Distance(transform.position, ray2D.point);
                    lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));

                    // Calculate default beam proportion multiplier based on default scale and current length
                    propMult = beamLength*(beamScale/10f);
                    // Spawn prefabs and apply force
                    owningWeaponSystem.Impact(ray2D.point);
                    hit = true;
                }
                // Nothing was his
                else
                {
                    // Set beam to maximum length
                    beamLength = MaxBeamLength;
                    lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                }
            }

            // Set beam scaling according to its length
            lineRenderer.material.SetTextureScale("_BaseMap", new Vector2(propMult, 1f));
        }

        // Advance texture frame
        void OnFrameStep()
        {
            Debug.Log("Frame stepping");
            // Set current texture frame based on frame counter
            lineRenderer.material.SetTexture("_BaseMap", BeamFrames[frameNo]);
            frameNo++;

            // Reset frame counter
            if (frameNo == BeamFrames.Length)
            {
                frameNo = 0;
            }
                
        }

        // Initialize frame animation
        void Animate()
        {
            if (BeamFrames.Length > 1)
            {
                // Set current frame
                frameNo = 0;
                lineRenderer.material.SetTexture("_BaseMap", BeamFrames[frameNo]);

                // Add timer 
                FrameTimerID = TimeManager.instance.AddTimer(FrameStep, BeamFrames.Length - 1, OnFrameStep);

                frameNo = 1;
            }
        }

        private float animateUVTime;

        void Update()
        {
            // Animate texture UV
            if (AnimateUV)
            {
                animateUVTime += Time.deltaTime;

                if (animateUVTime > 1.0f)
                    animateUVTime = 0f;

                lineRenderer.material.SetTextureOffset("_BaseMap", new Vector2(animateUVTime * UVTime + initialBeamOffset, 0f));
            }

            if (hit) return;
                // Raycast for laser beams
            Raycast();
        }
    }
}