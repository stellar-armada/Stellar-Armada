using UnityEngine;

public class MenuView : MonoBehaviour
{
    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
