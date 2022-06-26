using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyUnity
{
    public class ResMgr : Singleton<ResMgr>
    {
        public GameObject CreateGO(string path)
        {
            var obj = Instantiate(Resources.Load<GameObject>(path));
            obj.name = obj.name.Replace("(Clone)", "");
            return obj;
        }

        public T CreateCom<T>(string path) where T : UnityEngine.Component
        {
            var obj = Instantiate(Resources.Load<GameObject>(path));
            obj.name = obj.name.Replace("(Clone)", "");
            return obj.GetComponent<T>();
        }

        public T CreateRes<T>(string path) where T:Object
        {
            var obj = Instantiate(Resources.Load(path));
            return (T)obj;
        }
    }
}