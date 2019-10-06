using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameManager : MonoBehaviour
{
    public static string name = "";
    [SerializeField] InputField inputField;

    void Start()
    {
        inputField.text = name;
    }

    public void SetNameAndLoadScene()
    {
        name = inputField.text;
        SceneManager.LoadScene(1);
    }
}
