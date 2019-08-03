using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textDisplay;
    
    private string systemOnline = "SYSTEM ONLINE";
    private string systemOffLine = "SYSTEM OFFLINE";

    private char[] systemOnlineChar;
    private char[] systemOfflineChar;

    [SerializeField] float characterSpeed = .1f;
    [SerializeField] private float holdTime = 2f;

    public static SystemUIController instance;

    public void ShowSystemOnline()
    {
        StartCoroutine(DisplayText(systemOnlineChar));
        textDisplay.color = Color.green;
    }

    public void ShowSystemOffline()
    {
        StartCoroutine(DisplayText(systemOfflineChar));
        textDisplay.color = Color.red;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        systemOnlineChar = systemOnline.ToCharArray();
        systemOfflineChar = systemOffLine.ToCharArray();
    }

    IEnumerator DisplayText(char[] text)
    {
        float timer = 0;
        for (int i = 0; i < text.Length; i++)
        {
            textDisplay.text += text[i];
            yield return new WaitForSeconds(characterSpeed);
        }

        yield return new WaitForSeconds(holdTime);
        textDisplay.text = "";

    }
}
