using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    /// <summary>
    /// Mirror辅助类
    /// </summary>
    public static class MHelper
    {
        public static T LookupServerNid<T>(uint nid)
            where T : UnityEngine.Object
        {
            if (NetworkServer.spawned.TryGetValue(nid, out NetworkIdentity compId))
            {
                return compId.GetComponent<T>();
            }
            return null;
        }

        public static T LookupClientNid<T>(uint nid)
            where T : UnityEngine.Object
        {
            if (NetworkClient.spawned.TryGetValue(nid, out NetworkIdentity compId))
            {
                return compId.GetComponent<T>();
            }
            return null;
        }

        public static uint GetNid(this Component self)
        {
            var compId = self.GetComponent<NetworkIdentity>();
            if(compId==null) return 0;
            return compId.netId;
        }

        public static uint GetNid(this GameObject self)
        {
            var compId = self.GetComponent<NetworkIdentity>();
            if (compId == null) return 0;
            return compId.netId;
        }
    }
}