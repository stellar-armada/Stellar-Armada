using StellarArmada.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using Wacki;

// Switch event systems based on whether we are in desktop or VR mode
#pragma warning disable 0649
public class EventSystemSwitcher : MonoBehaviour
{
    [SerializeField] LaserPointerInputModule pointerInputModule;
    [SerializeField] StandaloneInputModule standaloneInputModule;
    
    void Start()
    {
        if (PlayerSettingsManager.GetDisplayMode())
        {
            pointerInputModule.enabled = false;
            standaloneInputModule.enabled = true;
        }
        else
        {
            pointerInputModule.enabled = true;
            standaloneInputModule.enabled = false;
        }
    }
}
