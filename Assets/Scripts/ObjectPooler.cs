using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string poolName;             // Name for identifying the pool
        public GameObject prefab;           // Prefab for this pool
        public int poolSize = 10;           // Initial pool size
    }

    [SerializeField] private List<Pool> pools; // List of different pools
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<GameObject, Transform> poolContainers;

    private void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        poolContainers = new Dictionary<GameObject, Transform>();

        foreach (Pool pool in pools)
        {
            CreatePool(pool);
        }
    }

    private void CreatePool(Pool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        GameObject poolContainer = new GameObject($"Pool - {pool.prefab.name}");
        poolContainers[pool.prefab] = poolContainer.transform;

        for (int i = 0; i < pool.poolSize; i++)
        {
            GameObject newInstance = Instantiate(pool.prefab);
            newInstance.transform.SetParent(poolContainer.transform);
            newInstance.SetActive(false);
            objectPool.Enqueue(newInstance);
        }

        poolDictionary[pool.prefab] = objectPool;
    }

    public GameObject GetInstanceFromPool(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"Creating a new pool dynamically for prefab: {prefab.name}. Consider preloading this prefab.");
            CreatePool(new Pool { prefab = prefab, poolSize = 10 });
        }

        Queue<GameObject> objectPool = poolDictionary[prefab];

        if (objectPool.Count > 0)
        {
            GameObject instance = objectPool.Dequeue();
            instance.SetActive(true);
            return instance;
        }
        else
        {
            Debug.LogWarning($"Pool for {prefab.name} is empty. Consider increasing initial pool size.");
            GameObject newInstance = Instantiate(prefab);
            newInstance.transform.SetParent(poolContainers[prefab]);
            return newInstance;
        }
    }


    public void ReturnToPool(GameObject prefab, GameObject instance)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"No pool exists for prefab {prefab.name}. Cannot return instance to pool.");
            
            instance.SetActive(false); // Deactivate, don't destroy
            return;
        }

        instance.SetActive(false);
        poolDictionary[prefab].Enqueue(instance);
    }
}
