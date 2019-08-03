// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using UnityEngine;
using UnityEngine.UI;

public class ServerStatusSlot : MonoBehaviour
{
    public TMPro.TextMeshProUGUI serverNameText;
    public TMPro.TextMeshProUGUI mapText;
    public TMPro.TextMeshProUGUI sizeText;
    public TMPro.TextMeshProUGUI playersText;
    public TMPro.TextMeshProUGUI latencyText;
    public TMPro.TextMeshProUGUI addressText;
    public string localAddress;
    public Button selectObjectButton;
}
