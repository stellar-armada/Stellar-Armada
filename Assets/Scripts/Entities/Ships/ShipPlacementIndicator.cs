using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipPlacementIndicator : MonoBehaviour
    {
        public NetworkEntity entity;
        public ShipSelectionHandler selectionHandler;
        public GameObject visualModel;
        [SerializeField] private LineRenderer lineRenderer;
        public float lineRendererWidth = .1f;
        
        private Transform t;
        private Transform miniMapEntityTransform;
        
        void Awake()
        {
            t = transform;
            ShipPlacementUIManager.instance.placementIndicators.Add(this);
        }

        void Start()
        {
            miniMapEntityTransform = entity.miniMapEntity.transform;
            t.SetParent(ShipPlacementCursor.instance.transform);
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            Hide();
        }

        public void Show(Vector3 pos)
        {
            // position the placement indicator in local space
            t.localPosition = pos;
            visualModel.SetActive(true);
        }

        public void Hide()
        {
            visualModel.SetActive(false);
        }

        void LateUpdate()
        {
            if (selectionHandler.isSelected)
            {
                lineRenderer.SetPositions(new[]
                {
                    t.position,
                    miniMapEntityTransform.position
                });
                lineRenderer.widthMultiplier = lineRendererWidth;
            }
            else // Fixes a bug where sometimes the indicator would not disable (possibly on stop all?)
            {
                if (visualModel.activeSelf)
                {
                    visualModel.SetActive(false);
                }
            }
        }
    }
}