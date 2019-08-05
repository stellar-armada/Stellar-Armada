using UnityEngine;
using UnityEngine.UI;
namespace SpaceCommander.UI
{
    public class ScoreboardPlayerObject : MonoBehaviour
    {
        public string netId;

        [SerializeField] TMPro.TextMeshProUGUI nameObject;
        [SerializeField] Image backgroundImage;
        
        public void SetBackgroundColor(Color color)
        {
            backgroundImage.color = color;
        }

        public void SetName(string name)
        {
            nameObject.text = name;
        }
        public void Init(string netIdOfOwningPlayer, string name)
        {
            netId = netIdOfOwningPlayer;
            nameObject.text = name;
        }
    }
}