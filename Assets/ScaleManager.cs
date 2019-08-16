using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    public static float scale = 1f;

    public float startScale;

    public delegate void ScaleEventHandler(float newScale);

    public static ScaleEventHandler OnScaleEventHandler;

    void Awake()
    {
        OnScaleEventHandler = null; // clear delegates
        SetScale(startScale);
    }
    
    public static void SetScale(float newScale)
    {
        Shader.SetGlobalFloat("WorldScale", newScale);
        scale = newScale;
        OnScaleEventHandler?.Invoke(newScale);
    }

    public static float GetScale() => scale;
}
