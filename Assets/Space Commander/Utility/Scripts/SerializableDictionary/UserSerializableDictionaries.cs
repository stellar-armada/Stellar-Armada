using System;
using SpaceCommander.Ships;
using SpaceCommander.Weapons;
using UnityEngine;

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class WeaponAudioClipDictionary : SerializableDictionary<WeaponType, EnumeratedAudioClip>
{
}

[Serializable]
public class WeaponPrefabDictionary : SerializableDictionary<WeaponType, EnumeratedWeaponPrefab>
{
}

[Serializable]
public class FleetDictionary : SerializableDictionary<ShipType, int>
{
}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}