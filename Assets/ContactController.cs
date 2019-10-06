using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactController : MonoBehaviour
{
    public static ContactController instance;

    [SerializeField] Renderer ren;
    
    [SerializeField] private string shaderAttributeName = "_UnlitColor";

    [SerializeField] private float fadeTime = 1f;
    
    [SerializeField] private float holdTime = 5f;
    
    void Awake() => instance = this;
    
    void Start() => ren.sharedMaterial.SetColor(shaderAttributeName, new Color(0,0,0,0));

    public void ShowContact()
    {
        Debug.Log("Showing contact");
        StartCoroutine(FadeContactTo(1f));
    }
    
    IEnumerator FadeContactTo(float toVal)
    {
        float timer = 0;
        float currentVal = ren.sharedMaterial.GetColor(shaderAttributeName).a;
        
        while (timer < fadeTime)
        {
            float alpha = Mathf.Lerp(currentVal, toVal, timer / fadeTime);
            
            ren.sharedMaterial.SetColor(shaderAttributeName, new Color(1,1,1, alpha));
            timer += Time.deltaTime;
            yield return null;
        }
        
        ren.sharedMaterial.SetColor(shaderAttributeName, Color.white);

        yield return new WaitForSeconds(holdTime);
        
        CameraFadeController.instance.FadeOut();
    }
}
