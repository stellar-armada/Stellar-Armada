using StellarArmada.Levels;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Weapons
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

        [SerializeField] private float lineRendererThickness;

        [SerializeField] private Transform miniMapRepresentation;
        [SerializeField] LineRenderer miniMapLineRenderer;

        private bool hit;

        private Transform t;
        void Awake()
        {
            t = GetComponent<Transform>();
            // Get line renderer component
            lineRenderer = GetComponent<LineRenderer>();
            miniMapLineRenderer = miniMapRepresentation.GetComponent<LineRenderer>();
            // Assign first frame texture
            if (!AnimateUV && BeamFrames.Length > 0)
            {
                lineRenderer.material.SetTexture("_BaseMap", BeamFrames[0]);
            miniMapLineRenderer.material.SetTexture("_BaseMap", BeamFrames[0]);
            }
            lineRenderer.widthMultiplier = lineRendererThickness;
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
                
            miniMapRepresentation.gameObject.SetActive(true);
            miniMapRepresentation.SetParent(VRMiniMap.instance.transform);
            miniMapRepresentation.localScale = t.lossyScale;
            miniMapRepresentation.localPosition = t.position;
            miniMapLineRenderer.widthMultiplier = lineRendererThickness * VRMiniMap.instance.transform.lossyScale.x;
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
            // Reset frame counter
            frameNo = 0;

            // Clear timer
            if (FrameTimerID != -1)
            {
                TimeManager.instance.RemoveTimer(FrameTimerID);
                FrameTimerID = -1;
            }
            miniMapRepresentation.gameObject.SetActive(false);
            miniMapRepresentation.SetParent(transform);
        }

        // Hit point calculation
        void Raycast()
        { 
            // Prepare structure and create ray
            hitPoint = new RaycastHit();
            Ray ray = new Ray(t.position, t.forward);
            // Calculate default beam proportion multiplier based on default scale and maximum length
            float propMult = MaxBeamLength*beamScale;

            // Raycast
            if (Physics.Raycast(ray, out hitPoint, MaxBeamLength, layerMask))
            {
                // Get current beam length and update line renderer accordingly
                beamLength = Vector3.Distance(t.position, hitPoint.point);
                lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                miniMapLineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                
                // Calculate default beam proportion multiplier based on default scale and current length
                propMult = beamLength*beamScale;
                // Spawn prefabs and apply force
                owningWeaponSystem.Impact(hitPoint.point);
                hit = true;
            }
            else
                {
                    // Set beam to maximum length
                    beamLength = MaxBeamLength;
                    lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                    miniMapLineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                }

            // Set beam scaling according to its length
            lineRenderer.material.SetTextureScale("_BaseMap", new Vector2(propMult, 1f));
            miniMapLineRenderer.material.SetTextureScale("_BaseMap", new Vector2(propMult, 1f));
        }

        // Advance texture frame
        void OnFrameStep()
        {
            // Set current texture frame based on frame counter
            lineRenderer.material.SetTexture("_BaseMap", BeamFrames[frameNo]);
            miniMapLineRenderer.material.SetTexture("_BaseMap", BeamFrames[frameNo]);
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
                miniMapLineRenderer.material.SetTexture("_BaseMap", BeamFrames[frameNo]);
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
                miniMapLineRenderer.material.SetTextureOffset("_BaseMap", new Vector2(animateUVTime * UVTime + initialBeamOffset, 0f));
            }

            if (hit) return;
                // Raycast for laser beams
            Raycast();
        }
    }
}