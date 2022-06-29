using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using FancyCSharp;

namespace FancyUnity
{
    public class SpawnTreeFixer : NetworkBehaviour
    {


        [Server]
        public void Fix(Transform root)
        {
            //记录父子关系
            List<TreeNodeInfo> infos = new List<TreeNodeInfo>();
            root.ForEachDescendant(trans =>
            {
                var idComp = trans.GetComponent<NetworkIdentity>();
                if (idComp != null && trans.parent != null)
                {//需要记录挂接
                    Debug.Log("nid="+idComp.netId);
                    var parentSlot = trans.parent.GetComponent<LinkSlot>();
                    if (parentSlot == null)
                    {
                        Debug.LogWarning("有挂接点没有附加LinkSlot");
                        parentSlot = trans.parent.gameObject.AddComponent<LinkSlot>();
                        parentSlot.SlotName = trans.parent.name;
                        parentSlot.RootNid = trans.parent.GetComponentInParent<NetworkIdentity>().netId;
                    }
                    var node = new TreeNodeInfo();
                    node.ChildNid = idComp.netId;
                    node.LinkRootNid = parentSlot.RootNid;
                    node.ParentSlotName = parentSlot.SlotName;
                    node.LocalPos = trans.localPosition;
                    node.LocalEularAngles = trans.localRotation.eulerAngles;
                    node.LocalScale = trans.localScale;
                    infos.Add(node);
                }
            });
            //传输记录到客户端使其同步
            RpcUpdateTree(infos);
        }

        [ClientRpc]
        public void RpcUpdateTree(List<TreeNodeInfo> nodeInfos)
        {
            foreach (var n in nodeInfos)
            {
                var child = NetworkClient.spawned[n.ChildNid].transform;
                var root = NetworkClient.spawned[n.LinkRootNid].GetComponent<LinkRoot>();
                var transSlot = root.GetComponent<LinkRoot>().GetTransform(n.ParentSlotName);
                child.parent = transSlot;
                child.localPosition = n.LocalPos;
                child.localEulerAngles = n.LocalEularAngles;
                child.localScale = n.LocalScale;
            }
        }
    }

    public class TreeNodeInfo
    {
        /// <summary>
        /// 在客户端定位没有被挂接的子级根节点
        /// </summary>
        public uint ChildNid;
        //以下2个字段用于在客户端定位slot
        public uint LinkRootNid;
        /// <summary>
        /// 带有NetworkIdentity组件的对象，其父级不一定直接是带着NetworkIdentity组件。
        /// 所以另外增加一个带搜集查找功能的LinkSlot组件用于标记和序列化传输。
        /// 目的是给服务端父级提供查找父级对象的依据。
        /// </summary>
        public string ParentSlotName;

        //以下个字段用于在客户端同步几何变换
        public Vector3 LocalPos;
        public Vector3 LocalEularAngles;
        public Vector3 LocalScale;
    }
}
