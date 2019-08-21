using SpaceCommander;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    [HideInInspector] public uint entityId;
    [HideInInspector] public NetworkEntity entity;
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
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        visualModel.SetActive(true);
    }
    public void Hide() =>  visualModel.SetActive(false);
}
