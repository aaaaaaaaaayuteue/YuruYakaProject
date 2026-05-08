using Hotbar.Base;
using Hotbar.Type;
using System.Collections.Generic;
using UnityEngine;

namespace Hotbar.Manager
{
    public class ObjectPoolingManager : SingletonMonobase<ObjectPoolingManager>
    {
        public HotbarDictionary<string, GameObject> prefabs = new HotbarDictionary<string, GameObject>();
        public HotbarDictionary<string, List<GameObject>> poolDictionary = new();
        public Transform poolParent;

        public static void Do_Initialize(System.Action endEvent)
        {
            //Light
            Instance.Do_RegisterPool("Light1", Instance.prefabs["Light1"], 100);


            endEvent();
        }

        public void Do_RegisterPool(string key, GameObject prefab, int initialSize)
        {
            if (poolDictionary.ContainsKey(key))
                return;

            var objectPool = new List<GameObject>();
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.SetParent(poolParent);
                obj.SetActive(false);
                objectPool.Add(obj);
            }
            poolDictionary[key] = objectPool;
        }

        public GameObject Do_SpawnFromPool(string key, Vector3 position = default, Quaternion rotation = default)
        {
            if (!poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning("Pool with key " + key + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn;

            if (poolDictionary[key].Count == 0)
            {
                objectToSpawn = Instantiate(Instance.prefabs[key]);
            }
            else
            {
                objectToSpawn = poolDictionary[key][0];
                poolDictionary[key].RemoveAt(0);
            }

            objectToSpawn.transform.position = position == default ? objectToSpawn.transform.position : position;
            objectToSpawn.transform.rotation = rotation == default ? objectToSpawn.transform.rotation : rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public void Do_ReturnToPool(string key, GameObject obj)
        {
            obj.transform.SetParent(poolParent);
            obj.SetActive(false);
            poolDictionary[key].Add(obj);
        }
    }
}