using StellarArmada.UI;
using TMPro;
using UnityEngine;

public class FleetLockHelper : MonoBehaviour
{
    [SerializeField] Color lockedColor;
    [SerializeField] Color unlockedColor;
    [SerializeField] TextMeshProUGUI text;

    void Update()
    {
        if (GroupUIManager.instance.GroupShipsLocked())
            text.color = lockedColor;
        else
            text.color = unlockedColor;
    }
}