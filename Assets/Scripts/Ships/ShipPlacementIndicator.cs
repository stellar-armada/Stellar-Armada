using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipPlacementIndicator : MonoBehaviour
    {
        [FormerlySerializedAs("entity")] public Ship ship;
        public ShipSelectionHandler selectionHandler;
        public GameObject visualModel;
        [SerializeField] private LineRenderer lineRenderer;
        public float lineRendererWidth = .1f;
        
        private Transform t;
        [SerializeField] Transform transformToOriginateFrom;

        void Start()
        {
            t = transform;
            ShipPlacementUIManager.placementIndicators.Add(this);
            t.SetParent(ShipPlacementCursor.instance.transform);
            t.localRotation = Quaternion.identity;
            //t.localScale = Vector3.one;
            Hide();
        }

        public void Show(Vector3 pos)
        {
            // position the placement indicator in local space
            t.localPosition = pos;
            visualModel.SetActive(true);
            lineRenderer.enabled = true;
        }

        public void Hide()
        {
            visualModel.SetActive(false);
            lineRenderer.enabled = false;
        }

        void LateUpdate()
        {
            
            
            if (selectionHandler.isSelected)
            {
                lineRenderer.SetPositions(new[]
                {
                    t.position,
                    transformToOriginateFrom.position
                });
                var distance = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1));
                lineRenderer.materials[0].mainTextureScale = new Vector3(distance, 1, 1);
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