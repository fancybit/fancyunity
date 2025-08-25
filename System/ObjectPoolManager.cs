using System;
using System.Collections.Generic;
using UnityEngine;

namespace FancyUnity
{
    /// <summary>
    /// GameObject 对象池管理器
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        public PoolSize[] InitPoolsSize;

        protected Dictionary<string, Pool> poolTable
            = new Dictionary<string, Pool>();

        protected Dictionary<GameObject, Pool> poolTableObj
            = new Dictionary<GameObject, Pool>();

        protected override void Start()
        {
            base.Start();
            Perpetual = true;

            foreach (var p in InitPoolsSize)
            {
                try
                {
                    var pool = GetPool(p.Prefab);
                    pool?.SetCapacity(p.Size);
                }
                catch { }
            }
        }

        public Pool GetPool(string name)
        {
            if (poolTable.ContainsKey(name))
                return poolTable[name];
            return null;
        }

        public Pool GetPool(GameObject prefab)
        {
            if (!poolTableObj.TryGetValue(prefab, out Pool pool))
            {
                pool = prefab.GetComponent<Pool.PoolTag>()?.Pool;
            }
            if (pool == null)
            {
                pool = new Pool(prefab);
            }
            return pool;
        }

        public GameObject Get(GameObject prefab)
        {
            return GetPool(prefab)?.Get();
        }

        public List<GameObject> Get(GameObject prefab, long count)
        {
            return GetPool(prefab).Get(count);
        }

        public GameObject Get(string name)
        {
            return GetPool(name).Get();
        }

        public List<GameObject> Get(string prefabName, long count)
        {
            return GetPool(prefabName).Get(count);
        }


        public T GetComp<T>(GameObject prefab) where T : Component
        {
            return GetPool(prefab)?.Get()?.GetComponent<T>();
        }

        public List<T> GetComp<T>(GameObject prefab, long count) where T : Component
        {
            return GetPool(prefab)?.GetComp<T>(count);
        }

        public T GetComp<T>(string name) where T : Component
        {
            return GetPool(name).Get()?.GetComponent<T>();
        }

        public List<T> GetComp<T>(string prefabName, long count) where T : Component
        {
            return GetPool(prefabName)?.GetComp<T>(count);
        }

        public class Pool
        {
            protected Queue<GameObject> pool = new Queue<GameObject>();
            protected Transform mgrCon;

            public Pool(GameObject prefab)
            {
                Prefab = prefab;
                PrefabName = prefab.name;
                var mgr = ObjectPoolManager.Inst;
                mgr.poolTableObj.Add(prefab, this);
                mgr.poolTable.Add(prefab.name, this);
                mgrCon = new GameObject(prefab.name).transform;
                mgrCon.SetParent(mgr.transform);
                SetCapacity(10);
            }

            public string PrefabName;
            public GameObject Prefab;

            public void SetCapacity(long capacity)
            {
                if (capacity < pool.Count)
                {//缩容
                    for (var i = 0L; i < pool.Count - capacity; ++i)
                    {
                        var obj = pool.Dequeue();
                        GameObject.Destroy(obj);
                    }
                }
                else
                {//扩容
                    for (long i = 0; i < capacity - pool.Count; ++i)
                    {
                        var obj = GameObject.Instantiate(Prefab);
                        var info = obj.GetComponent<PoolTag>();
                        if (info == null)
                        {
                            info = obj.AddComponent<PoolTag>();
                        }
                        info.Pool = this;
                        Collect(obj);
                    }
                }
            }

            public void Collect(GameObject obj)
            {
                var poolInfo = obj.GetComponent<PoolTag>();
                if (poolInfo == null)
                {
                    poolInfo = obj.AddComponent<PoolTag>();
                }
                poolInfo.ResetToPool();
            }

            public void Collect(Component comp)
            {
                Collect(comp.gameObject);
            }

            public GameObject Get()
            {
                if (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    obj.SetActive(true);
                    return obj;
                }
                else
                {
                    var obj = GameObject.Instantiate(Prefab);
                    var poolable = obj.GetComponent<PoolTag>();
                    if (poolable == null)
                    {
                        poolable = obj.AddComponent<PoolTag>();
                    }
                    poolable.Pool = this;
                    return obj;
                }
            }

            public List<GameObject> Get(long count)
            {
                var result = new List<GameObject>();
                for (var i = 0L; i < count; ++i)
                {
                    result.Add(Get());
                }
                return result;
            }

            public T GetComp<T>() where T : Component
            {
                return Get()?.GetComponent<T>();
            }

            public List<T> GetComp<T>(long count) where T : Component
            {
                var result = new List<T>();
                for (var i = 0; i < count; ++i)
                {
                    result.Add(GetComp<T>());
                }
                return result;
            }


            public class PoolTag : MonoBehaviour
            {
                public Pool Pool;
                public virtual void ResetToPool()
                {
                    Pool.pool.Enqueue(gameObject);
                    gameObject.transform.SetParent(Pool.mgrCon);
                    gameObject.SetActive(false);
                }
            }
        }

        [Serializable]
        public struct PoolSize
        {
            public GameObject Prefab;
            public long Size;
        }
    }
}
