using StellarArmada.Match.Messaging;
using UnityEditor;
#pragma warning disable 0649
namespace StellarArmada
{
    [CustomPropertyDrawer((typeof(SfxDictionary)))]
    public class SfxPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(MessageDictionary)))]
    public class MessagePropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(WeaponAudioClipDictionary)))]
    public class WeaponAudioClipPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(WeaponPrefabDictionary)))]
    public class WeaponPrefabPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(ShipIdDictionary)))]
    public class FleetDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(ShipDictionary)))]
    public class ShipDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(MenuStateDictionary)))]
    public class MenuStateDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(WinConditionDictionary)))]
    public class WinConditionDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}