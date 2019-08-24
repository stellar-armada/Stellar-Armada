using StellarArmada;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    public uint entityId; 
    public NetworkEntity entity;
    public GameObject visualModel;
    [SerializeField] private LineRenderer lineRenderer;
    public float lineRendererWidth = .1f;

    private bool isActive;
    void Start()
    {
        transform.SetParent(PlacementCursor.instance.transform);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        Hide();
    }

    public void Show(Vector3 pos)
    {
        Transform t = transform;
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
            lineRenderer.SetPositions(new []
            {
                transform.position, 
                entity.mapEntity.transform.position
            });
            lineRenderer.widthMultiplier = lineRendererWidth;
        }
    }
}
