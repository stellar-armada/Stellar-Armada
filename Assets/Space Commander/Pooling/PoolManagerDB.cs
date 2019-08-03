using UnityEngine;
using System.Collections.Generic;
using System;

namespace SpaceCommander.Pooling
{
    [Serializable]
    public class PoolManagerDB : ScriptableObject
    { 
        public List<string> poolsName; 
        public List<PoolContainer> pools; 
        public string namer = "default";
    }
}
