using UnityEngine;

namespace  SpaceCommander.UI

{
public class VRCanvas : MonoBehaviour
{
    public static VRCanvas instance;

    public Canvas canvas;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        canvas = GetComponent<Canvas>();
    }
}
}