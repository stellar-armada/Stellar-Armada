using StellarArmada.Levels;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityMovementIndicator : MonoBehaviour
    {
        [SerializeField] EntityMovement movement;
        private bool isActive = false;
        public GameObject visualModel;
        [SerializeField] LineRenderer lineRenderer;
        public float lineRendererWidth = .1f;

        [SerializeField] Transform entityTransform;

        private Transform t;
        void Awake()
        {
            t = transform;
            visualModel.SetActive(false);
        }

        void Start()
        {
            // Subscribe to on ship start move (Vector 3, Quaternion)
            movement.OnMoveToPoint += ShowMovementIndicator;
            // Subscribe to on ship stop move
            movement.OnArrival += HideMovementIndicator;
        }

        void LateUpdate()
        {
            if (isActive)
            {
                lineRenderer.SetPositions(new[]
                {
                    t.position,
                    entityTransform.position
                });
                lineRenderer.widthMultiplier = lineRendererWidth;
            }
        }

        void ShowMovementIndicator(Vector3 pos, Quaternion rot)
        {
            isActive = true;
            t.SetParent(VRMiniMap.instance.transform);
            t.localPosition = pos;
            t.localRotation = rot;
            t.localScale = Vector3.one;
            visualModel.SetActive(true);
            lineRenderer.enabled = true;
        }

        void HideMovementIndicator()
        {
            isActive = false;
            visualModel.SetActive(false);
            lineRenderer.enabled = false;
        }

    }
}