using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Audio {
    

    [System.Serializable, CreateAssetMenu(fileName = nameof(SfxAudioData), menuName = "SFX Data", order = 52)]    
    public class SfxAudioData : ScriptableObject
    {
        [SerializeField] public SfxDictionary audioClips;
    }
}