using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipPlacementIndicator : MonoBehaviour
    {
        public uint entityId;
        public NetworkEntity entity;
        public GameObject visualModel;
        [SerializeField] private LineRenderer lineRenderer;
        public float lineRendererWidth = .1f;

        private bool isActive;

        private Transform t;
        private Transform miniMapEntityTransform;
        void Awake()
        {
            t = transform;
           
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
            isActive = true;
        }

        public void Hide()
        {
            isActive = false;
            visualModel.SetActive(false);
        }

        void LateUpdate()
        {
            if (isActive)
            {
                lineRenderer.SetPositions(new[]
                {
                    t.position,
                    miniMapEntityTransform.position
                });
                lineRenderer.widthMultiplier = lineRendererWidth;
            }
        }
    }
}