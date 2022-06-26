using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    /// <summary>
    /// GameObject 对象池
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool>
    {
        /// <summary>
        /// 对象的资源路径为KEY
        /// queue循环队列来引用池中对象
        /// </summary>
        protected Dictionary<string, Queue<GameObject>> pools
            = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// 设置某个对象池的预备容量
        /// 会根据原有和目标容量缩容或者扩容
        /// </summary>
        /// <param name="resPath">对象池原型物体的资源路径</param>
        /// <param name="capacity">对象池的容量</param>
        public void SetCapacity(string resPath, long capacity = 100L)
        {
            if (!pools.TryGetValue(resPath, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                pools.Add(resPath, pool);
            }
            if (capacity < pool.Count)
            {//缩容
                for (var i = 0L; i < pool.Count - capacity; ++i)
                {
                    var obj = pool.Dequeue();
                    Destroy(obj);
                }
            }
            else
            {//扩容
                for (long i = 0; i < capacity - pool.Count; ++i)
                {
                    var obj = ResMgr.Inst.CreateGO(resPath);
                    var info = obj.GetComponent<PoolTag>();
                    if (info == null)
                    {
                        info = obj.AddComponent<PoolTag>();
                    }
                    info.ResPath = resPath;
                    info.Pool = pool;
                    Collect(obj);
                }
            }
        }

        /// <summary>
        /// 从池中获取单个GameObject并返回它上面的某个Component
        /// </summary>
        /// <typeparam name="T">Component类型</typeparam>
        /// <param name="resPath">资源路径</param>
        /// <returns>获取的单个组件</returns>
        public T GetCom<T>(string resPath) where T : Component
        {
            return GetCom<T>(resPath, 1L)[0];
        }

        /// <summary>
        /// 从池中获取一组GameObject，获取上面的某个Component并返回一个列表
        /// </summary>
        /// <typeparam name="T">要获取的组件类型</typeparam>
        /// <param name="resPath">资源路径</param>
        /// <param name="count">要获取的数量</param>
        /// <returns>获取到的Component列表</returns>
        public List<T> GetCom<T>(string resPath, long count) where T : Component
        {
            if (!pools.TryGetValue(resPath, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                pools.Add(resPath, pool);
            }
            var result = new List<T>((int)count);
            for (var i = 0L; i < count; ++i)
            {
                if (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    obj.SetActive(true);
                    result.Add(obj.GetComponent<T>());
                }
                else
                {
                    var obj = ResMgr.Inst.CreateGO(resPath);
                    var poolable = obj.GetComponent<PoolTag>();
                    if (poolable == null)
                    {
                        poolable = obj.AddComponent<PoolTag>();
                    }
                    poolable.Pool = pool;
                    poolable.ResPath = resPath;
                    result.Add(obj.GetComponent<T>());
                }
            }
            return result;
        }

        /// <summary>
        /// 从对象池获取单个GameObject
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <returns></returns>
        public GameObject Get(string resPath)
        {
            return Get(resPath, 1)[0];
        }


        /// <summary>
        /// 从池中获取一组GameObject
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="count">要获取的数量</param>
        /// <returns>获取到的GameObject列表</returns>
        public List<GameObject> Get(string resPath, long count)
        {
            if (!pools.TryGetValue(resPath, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                pools.Add(resPath, pool);
            }
            var result = new List<GameObject>((int)count);
            for (var i = 0L; i < count; ++i)
            {
                if (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    obj.SetActive(true);
                    result.Add(obj);
                }
                else
                {
                    var obj = ResMgr.Inst.CreateGO(resPath);
                    var poolable = obj.GetComponent<PoolTag>();
                    if (poolable == null)
                    {
                        poolable = obj.AddComponent<PoolTag>();
                    }
                    poolable.Pool = pool;
                    poolable.ResPath = resPath;
                    result.Add(obj);
                }
            }
            return result;
        }

        /// <summary>
        /// 回收一个gameObject对象到对象池
        /// </summary>
        /// <param name="obj">被回收的GameObject</param>
        public void Collect(GameObject obj)
        {
            var poolInfo = obj.GetComponent<PoolTag>();
            if (poolInfo == null)
            {
                poolInfo = obj.AddComponent<PoolTag>();
            }
            poolInfo.ResetToPool();
        }

        /// <summary>
        /// 回收一个Component的宿主GameObject到对象池
        /// </summary>
        /// <param name="comp">要被回收的Component</param>
        public void Collect(Component comp)
        {
            Collect(comp.gameObject);
        }
    }
}
