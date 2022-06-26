using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class PoolTag:MonoBehaviour
    {
        public string ResPath;
        public Queue<GameObject> Pool;
        public virtual void ResetToPool()
        {
            Pool.Enqueue(gameObject);
            gameObject.transform.SetParent(ObjectPool.Inst.transform);
            gameObject.SetActive(false);
        }
    }
}
