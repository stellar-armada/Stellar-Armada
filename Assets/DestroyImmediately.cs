using UnityEngine;
public class DestroyImmediately : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_ANDROID
    void Start() => Destroy(gameObject);
#endif

}
