using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PhoneScreenController : MonoBehaviour
{

    [SerializeField] Renderer ren;

    [SerializeField] Light phoneLight;

    [SerializeField] private string shaderAttributeName = "_UnlitColor";

    [SerializeField] private float fadeTime = .4f;
    
    float lightStart;
    
    // Start is called before the first frame update
    void Start()
    {
        lightStart = phoneLight.intensity;
        phoneLight.intensity = 0;
        phoneLight.enabled = false;
        ren.sharedMaterial.SetColor(shaderAttributeName, new Color(1, 1, 1, 0));
    }
    
    

    public void ShowPhoneLight()
    {
        StartCoroutine(FadeLightTo(1f));
    }

    public void HidePhoneLight()
    {
        StartCoroutine(FadeLightTo(0f));
    }

    IEnumerator FadeLightTo(float toVal)
    {
        float timer = 0;
        float currentVal = ren.sharedMaterial.GetColor(shaderAttributeName).a;

        if (toVal.Equals(1f))
        {
            phoneLight.enabled = true;
        }
        
        while (timer < fadeTime)
        {
            float alpha = Mathf.Lerp(currentVal, toVal, timer / fadeTime);
            
            ren.sharedMaterial.SetColor(shaderAttributeName, Color.white * alpha);
            phoneLight.intensity = lightStart * alpha;
            timer += Time.deltaTime;
            yield return null;
        }
        
        ren.sharedMaterial.SetColor(shaderAttributeName, Color.white * toVal);
        phoneLight.intensity = lightStart;
        
        if (toVal.Equals(0f))
        {
            phoneLight.enabled = false;
        }
        
    }
    
}
