using UnityEngine; 
using System.Collections.Generic;
using System;

#pragma warning disable 0649
namespace SpaceCommander.Pooling
{
    [Serializable]
    public class PoolManager : MonoBehaviour
    { 
        public static PoolManager instance;                                              // instance of current manager 
        public static Dictionary<string, Pool> Pools = new Dictionary<string, Pool>();

        public string databaseName = "";                                                    // Name of database. Need for editor 
        public int selectedItem = 0;                                                        // Editor's parameter 
        public bool[] haveToShowArr;                                                        // Editor's parameter               

        List<Pool> curPools = new List<Pool>();                                       // Our pools 

        void Awake()
        {
            InstallManager();
            instance = this;
        }

        //Retirning pool by it's name
        public Pool GetPool(string valueName)
        {
            int i;
            if (valueName != "" && curPools != null && curPools.Count > 0)
            {
                for (i = 0; i < curPools.Count; i++)
                {
                    if (curPools[i].poolName == valueName)
                    {
                        return curPools[i];
                    }
                }
            }
            return null;
        }

        //Installing of manager
        void InstallManager()
        {
            curPools.Clear();
            List<PoolContainer> pools = new List<PoolContainer>();
            Pools.Clear();
            Pools = new Dictionary<string, Pool>();
            PoolManagerDB myManager = Resources.Load("PoolManagerCache/" + databaseName) as PoolManagerDB;
            if (myManager != null)
            {
                if (myManager.pools != null)
                {
                    foreach (PoolContainer cont in myManager.pools)
                    {
                        pools.Add(cont);
                    }
                }
            } 

            //Transfering info from containers to our real pools and creating GO's for them
            int j;
            for (j = 0; j < pools.Count; j++)
            {
                GameObject newGO = new GameObject();
                newGO.transform.parent = this.gameObject.transform;
                newGO.name = pools[j].poolName;

                Pool newPool = newGO.AddComponent<Pool>();
                newPool.poolName = newGO.name;
                newPool.SetTemplates(pools[j].templates);
                newPool.SetLength(pools[j].poolLength);
                newPool.SetLengthMax(pools[j].poolLengthMax);
                newPool.needBroadcasting = pools[j].needBroadcasting;
                newPool.broadcastSpawnName = pools[j].broadcastSpawnName;
                newPool.broadcastDespawnName = pools[j].broadcastDespawnName;
                newPool.needSort = pools[j].needSort;
                newPool.delayedSpawnInInstall = pools[j].delayedSpawnInInstall;
                newPool.objectsPerUpdate = pools[j].objectsPerUpdate;
                newPool.optimizeSpawn = pools[j].optimizeSpawn;
                newPool.targetFPS = pools[j].targetFPS;
                newPool.needSort = pools[j].needSort;
                newPool.needParenting = pools[j].needParenting;
                newPool.needDebugging = pools[j].needDebug;
                newPool.pooling = true;
                newPool.Install();

                curPools.Add(newPool);
                Pools.Add(newPool.name, newPool);

            }
        }
         
        public int GetPoolsCount()
        {
            if (curPools != null)
                return curPools.Count;
            return -1;
        }
    }
}