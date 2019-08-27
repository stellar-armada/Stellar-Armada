using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Audio {
// Scriptable object containing music data
    [System.Serializable, CreateAssetMenu(fileName = nameof(MusicData), menuName = "Music Data", order = 53)]    
    public class MusicData : ScriptableObject
    {
        [SerializeField] public AudioClip titleTrack;
        [SerializeField] public AudioClip[] gameTracks;
    }
}