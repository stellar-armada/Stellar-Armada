using UnityEngine;
namespace SpaceCommander.Player {
public class LookAtTarget : MonoBehaviour
{
    private Transform t;
    private Transform lT;

    private bool isInited;
    private Vector3 pos;
    void OnEnable()
    {
        PlayerController.EventOnLocalNetworkPlayerCreated += Init;
    }
    void Init()
    {
        if (LookTarget.instance == null)
        {
            enabled = false;
            return;
        }
        isInited = true;
        t = transform;
        lT = LookTarget.instance.transform;
    }

    void Update()
    {
        LateUpdate();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isInited) return;
        pos = lT.position;
        t.LookAt(new Vector3(pos.x, pos.y, pos.z));
    }
}
}