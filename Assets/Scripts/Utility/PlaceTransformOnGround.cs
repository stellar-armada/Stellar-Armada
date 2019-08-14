using UnityEngine;
#pragma warning disable 0649
namespace SpaceCommander.Utility
{
    // Helper script to ground an object onto the floor

    public class PlaceTransformOnGround : MonoBehaviour
    { 
        [SerializeField] Transform transformToPlace;

        [SerializeField] LayerMask layerMask;

        [SerializeField] float raycastHeight = 5f;

        [SerializeField] float maxDistance = 10f;

        RaycastHit hit;

        Vector3 positionAbove;

        // Update is called once per frame
        void Update()
        {
            positionAbove = new Vector3(transformToPlace.position.x, transformToPlace.position.y + raycastHeight, transformToPlace.position.z);
            if (Physics.Raycast(positionAbove, Vector3.down, out hit, maxDistance, layerMask))
            {
                transformToPlace.position = new Vector3(transformToPlace.position.x, hit.point.y, transformToPlace.position.z);
            }
        }
    }
}