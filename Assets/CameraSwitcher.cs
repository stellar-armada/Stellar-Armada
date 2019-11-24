using UnityEngine;

[System.Serializable]
public class PlatformGameObjectDictionary : SerializableDictionary<PlatformManager.PlatformType, GameObject>{}
public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private PlatformGameObjectDictionary platformGameObjectDictionary;
    
    void Start()
    {
        foreach (var key in platformGameObjectDictionary.Keys)
            platformGameObjectDictionary[key].SetActive(key == PlatformManager.instance.Platform);
    }
}
