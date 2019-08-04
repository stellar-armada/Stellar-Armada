using SpaceCommander.Audio;
using SpaceCommander.UI;
using UnityEditor;
namespace SpaceCommander
{
    [CustomPropertyDrawer((typeof(SfxDictionary)))]
    public class SfxPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(MessageDictionary)))]
    public class MessagePropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(WeaponAudioClipDictionary)))]
    public class WeaponAudioClipPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(WeaponPrefabDictionary)))]
    public class WeaponPrefabPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(FleetDictionary)))]
    public class FleetDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}