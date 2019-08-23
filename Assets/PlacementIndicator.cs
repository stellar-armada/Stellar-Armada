using StellarArmada;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    public uint entityId; 
    public NetworkEntity entity;
    public GameObject visualModel;

    void Start()
    {
        transform.SetParent(Placer.instance.transform);
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
    }
    public void Hide() =>  visualModel.SetActive(false);
}
