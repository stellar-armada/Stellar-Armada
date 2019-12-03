using UnityEngine;

[System.Serializable]
public class PlatformGameObjectDictionary : SerializableDictionary<PlatformManager.PlatformType, GameObject>{}
public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private PlatformGameObjectDictionary platformGameObjectDictionary;
    
    void Start()
    {
        foreach (var key in platformGameObjectDictionary.Keys)
            if (key == PlatformManager.instance.Platform)
                Instantiate(platformGameObjectDictionary[key], transform);
    }
}
