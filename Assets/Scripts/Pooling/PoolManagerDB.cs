using UnityEngine;
using System.Collections.Generic;
using System;

#pragma warning disable 0649
namespace StellarArmada.Pooling
{
    [Serializable]
    public class PoolManagerDB : ScriptableObject
    { 
        public List<string> poolsName; 
        public List<PoolContainer> pools; 
        public string namer = "default";
    }
}
