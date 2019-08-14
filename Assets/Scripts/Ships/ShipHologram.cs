using UnityEngine;
#pragma warning disable 0649
namespace  SpaceCommander.Ships
{
public class ShipHologram : MonoBehaviour
{
    private Renderer ren;
    [SerializeField] private Ship ship;
    void Awake()
    {
        ren = GetComponent<Renderer>();
        ren.enabled = false;
        GetComponent<MeshFilter>().sharedMesh = ship.visualModel.GetComponent<MeshFilter>().sharedMesh;
    }
}
}
