using StellarArmada.UI;
using TMPro;
using UnityEngine;

public class FleetLockHelper : MonoBehaviour
{
    [SerializeField] Color lockedColor;
    [SerializeField] Color unlockedColor;
    [SerializeField] TextMeshProUGUI text;

    public void ToggleLock()
    {
        if (GroupUIManager.instance.GroupShipsLocked())
        {
            GroupUIManager.instance.UnlockGroupShips();
            text.color = unlockedColor;
        }
        else
        {
            GroupUIManager.instance.LockGroupShips();
            text.color = lockedColor;
        }
    }

    void Start()
    {
        if (GroupUIManager.instance.GroupShipsLocked())
            text.color = lockedColor;
        else
            text.color = unlockedColor;
    }

}