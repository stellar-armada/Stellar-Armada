using SpaceCommander;
using SpaceCommander.Ships;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    public uint entityId;
    public NetworkEntity entity;
    public GameObject visualModel;

    void Awake()
    {
        transform.SetParent(Placer.instance.transform);
        Hide();
    }

    public void Show(Vector3 pos)
    {
        Transform t = transform;
        // position the placement indicator in local space
        t.localPosition = pos;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
        visualModel.SetActive(true);
    }
    public void Hide() =>  visualModel.SetActive(false);
}
