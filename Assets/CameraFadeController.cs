using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFadeController : MonoBehaviour
{
    public static CameraFadeController instance;

    [SerializeField] private Renderer fadeSphere;

    private float fadeTime = 3f;

    [SerializeField] private string shaderColorInputName = "_UnlitColor";

    [SerializeField] bool loadSceneZeroOnFadeOut = true;

    void Awake() => instance = this;
    
    void Start()
    {
        Invoke(nameof(FadeIn), 3f);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeTo(4,0));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeTo(4,1));
    }

    IEnumerator FadeTo(float waitSeconds, float toVal)
    {
        yield return new WaitForSeconds(waitSeconds);
        
        float currentVal = 1 - toVal;

        float timer = 0f;

        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            fadeSphere.material.SetColor(shaderColorInputName, new Color(0,0,0,Mathf.Lerp(currentVal, toVal, timer/fadeTime)));
            yield return null;
        }
        fadeSphere.material.SetColor(shaderColorInputName, new Color(0,0,0,toVal));

        yield return null;
        if (loadSceneZeroOnFadeOut && toVal.Equals(1f)) SceneManager.LoadScene(0);
    }
    
}
