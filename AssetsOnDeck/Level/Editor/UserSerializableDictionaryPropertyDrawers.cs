using SpaceCommander.Audio;
using SpaceCommander.Level;
using SpaceCommander.UI;
using UnityEditor;
namespace SpaceCommander
{
    // Required for serializable dictionaries
    [CustomPropertyDrawer(typeof(LevelDictionary))]
    public class LevelSizePropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer(typeof(ShipDictionary))]
    public class ShipPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(SfxDictionary)))]
    public class SfxPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    
    [CustomPropertyDrawer((typeof(MessageDictionary)))]
    public class MessagePropertyDrawer : SerializableDictionaryPropertyDrawer { }

}