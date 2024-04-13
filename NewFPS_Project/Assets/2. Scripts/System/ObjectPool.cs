using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix;
using Sirenix.Serialization;

public class ObjectPool : SingleTon<ObjectPool>
{
    public static int initialPoolSize = 50;
    
    [System.Serializable]
    public class PoolItem
    {
        public int size; // 풀 사이즈
        public GameObject prefab; // 오브젝트 프리팹
    }
    
    // 키는 프리팹, 값은 해당 프리팹의 인스턴스 큐.
    [NonSerialized ,OdinSerialize]
    public Dictionary<GameObject, Queue<GameObject>> pooledObjects = new Dictionary<GameObject, Queue<GameObject>>();
    // 현재 활성화된 오브젝트와 그 원본 프리팹을 매핑하는 사전.
    [NonSerialized ,OdinSerialize]
    public Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();
    
    public PoolItem[] Pools;
    
    // 싱글톤 초기화 및 오브젝트 풀 비동기적 생성
    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }
    
    private void InitializePools()
    {
        StartCoroutine(CreatePoolsCo());
    }
    
    // 오브젝트 풀 생성
    private IEnumerator CreatePoolsCo()
    {
        Debug.Log("New Pool Make");
        foreach (var pool in Pools)
        {
            CreatePool(pool.prefab, pool.size); // 각 프리팹에 대한 풀을 생성
            yield return null; 
        }
    }

    // 지정된 프리팹으로 오브젝트 풀을 생성
    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (!Instance.pooledObjects.ContainsKey(prefab))
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                var newObj = Instantiate(prefab);
                newObj.SetActive(false);
                queue.Enqueue(newObj);  // 큐에 오브젝트 추가
            }
            Instance.pooledObjects.Add(prefab, queue);
        }
    }
    
    // 지정된 프리팹으로 오브젝트를 스폰
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 풀을 확인하고, 없으면 생성
        if (!Instance.pooledObjects.ContainsKey(prefab))
        {
            CreatePool(prefab, initialPoolSize);
        }
        
        // 풀에서 사용할 오브젝트를 가져오기 위해 시도
        if (Instance.pooledObjects.TryGetValue(prefab, out Queue<GameObject> list) && list.Count > 0)
        {
            GameObject obj = list.Dequeue();            // 큐에서 오브젝트를 꺼냄
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            
            obj.SetActive(true);
            
            Instance.spawnedObjects.Add(obj, prefab);   // 스폰된 오브젝트 목록에 추가
            
            return obj;
        }
        else
        {   // 풀에 사용 가능한 오브젝트가 없다면 새로 생성
            Debug.Log("풀에서 사용할 오브젝트가 부족합니다.");
            var obj = Instantiate(prefab, position, rotation);
            Instance.spawnedObjects.Add(obj, prefab);   // 스폰된 오브젝트 목록에 추가
            return obj;
        }
    }
    
    // 사용이 끝난 오브젝트를 풀로 반환
    public static void Unspawn(GameObject obj)
    {
        // 오브젝트가 스폰된 오브젝트 목록에 있는지 확인
        if (Instance.spawnedObjects.TryGetValue(obj, out GameObject prefab))
        {
            Debug.Log($"Obj : {obj} // Prefeb : {prefab}");
            // 오브젝트를 비활성화하고 풀로 다시 반환
            obj.SetActive(false);
            Instance.spawnedObjects.Remove(obj);
            Instance.pooledObjects[prefab].Enqueue(obj);
         
            //obj.transform.SetParent(Instance.transform);
        }
        else
        {
            // 스폰된 목록에 없는 오브젝트라면 파괴
            Destroy(obj);
        }
    }
}
