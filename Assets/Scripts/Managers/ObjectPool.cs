using System.Collections.Generic;
using GameCore.Enemy;
using GameCore.Projectiles;
using Interfaces;
using UnityEngine;
using VContainer;

namespace Managers
{
    public class ObjectPool : MonoBehaviour, IObjectPoolService
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;
        private IObjectResolver _objectResolver;

        [Inject]
        public void Init(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        private void Awake()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
            foreach (var pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    
                    if (obj.TryGetComponent<Arrow>(out var arrow))
                    {
                        arrow.SetObjectResolver(_objectResolver);
                    }
                    
                    if (obj.TryGetComponent<EnemyController>(out var enemy))
                    {
                        enemy.SetObjectResolver(_objectResolver);
                    }
                    
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objectPool);
            }
        }
    
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }
        
            if (_poolDictionary[tag].Count == 0)
            {
                Debug.Log($"Pool {tag} is empty. Creating a new object.");
                GameObject newObj = Instantiate(GetPrefabByTag(tag));
                newObj.SetActive(false);
                _poolDictionary[tag].Enqueue(newObj);
            }
        
            GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
        
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
        
            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
            pooledObject?.OnObjectSpawn();

            return objectToSpawn;
        }
    
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return;
            }
        
            objectToReturn.SetActive(false);
            _poolDictionary[tag].Enqueue(objectToReturn);
        }
    
        private GameObject GetPrefabByTag(string tag)
        {
            foreach (var pool in pools)
            {
                if (pool.tag == tag)
                {
                    return pool.prefab;
                }
            }

            Debug.LogWarning($"Prefab with tag {tag} not found.");
            return null;
        }
    }
}