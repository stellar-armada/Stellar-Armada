using StellarArmada;
using UnityEngine;

public class MovementIndicator : MonoBehaviour
{
    [SerializeField] EntityMovement movement;
    public GameObject visualModel;

    void Start()
    {
        // Subscribe to on ship start move (Vector 3, Quaternion)
        movement.OnMoveToPoint += ShowPlacementIndicator;
        // Subscribe to on ship stop move
        movement.OnStop += HidePlacementIndicator;
    }

    void ShowPlacementIndicator(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        gameObject.SetActive(true);
    }

    void HidePlacementIndicator()
    {
        gameObject.SetActive(false);
    }

}
