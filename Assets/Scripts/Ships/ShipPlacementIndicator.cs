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
        }

        public void Hide()
        {
            visualModel.SetActive(false);
        }

        void LateUpdate()
        {
            
            
            if (selectionHandler.isSelected)
            {

                
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