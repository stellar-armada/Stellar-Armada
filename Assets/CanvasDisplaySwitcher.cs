using System.Collections;
using System.Collections.Generic;
using StellarArmada.Player;
using UnityEngine;

public class CanvasDisplaySwitcher : MonoBehaviour
{

    public Canvas canvas;
    public Vector3 scale3D;
    public Vector3 position3D;
    

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerSettingsManager.instance.desktopDisplayMode)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.transform.localScale = Vector3.one;
        }
        else
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.localScale = scale3D;
            canvas.transform.localPosition = position3D;
        }
    }
}
