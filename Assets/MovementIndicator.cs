using StellarArmada;
using UnityEngine;

public class MovementIndicator : MonoBehaviour
{
    [SerializeField] EntityMovement movement;
    private bool isActive = false;
    public GameObject visualModel;
    [SerializeField] LineRenderer lineRenderer;
    public float lineRendererWidth = .1f;

    [SerializeField] Transform entityTransform;

    void Awake()
    {
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
            lineRenderer.SetPositions(new []
            {
                transform.position, 
                entityTransform.position
            });
            lineRenderer.widthMultiplier = lineRendererWidth;
        }
    }

    void ShowMovementIndicator(Vector3 pos, Quaternion rot)
    {
        isActive = true;
        transform.SetParent(MiniMap.instance.transform);
        transform.localPosition = pos;
        transform.localRotation = rot;
        transform.localScale = Vector3.one;
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
