using SpaceCommander.Audio;
using SpaceCommander.Match.Messaging;
using UnityEditor;
#pragma warning disable 0649
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
    
    [CustomPropertyDrawer((typeof(ShipDictionary)))]
    public class ShipDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}