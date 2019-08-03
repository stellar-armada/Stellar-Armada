using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.UI.Mobile
{
    public class MenuButton : MonoBehaviour
    {
        private Button button;

        // Start is called before the first frame update
        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ToggleMenu);
        }

        void ToggleMenu()
        {
            if (PlayerCanvasController.instance.MenuIsActive())
            {
                PlayerCanvasController.instance.HideCanvas();
            }
            else
            {
                PlayerCanvasController.instance.ShowCanvas();

            }
        }
    }
}
