using UnityEngine;

namespace SpaceCommander.Player
{
    public class FootGrounder : MonoBehaviour
    {
        RaycastHit[] hits;
        [SerializeField] LayerMask layersToHit;

        // Update is called once per frame
        void Update()
        {
            Debug.DrawLine(transform.position + (Vector3.up * 10), transform.position - Vector3.up * 10, Color.red,
                .1f);
            hits = Physics.RaycastAll(transform.position + (Vector3.up * 10), -Vector3.up, 20, layersToHit);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name == "Floor")
                {
                    transform.position = hit.point;
                    return;
                }
            }
        }
    }
}
