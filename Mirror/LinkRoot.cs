using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class LinkRoot:MonoBehaviour
    {
        public Dictionary<string, LinkSlot> Slots = new Dictionary<string, LinkSlot>();
        protected NetworkIdentity NidComp;

        protected virtual void Awake()
        {
            Init();
        }

        public void Init()
        {
            NidComp = GetComponent<NetworkIdentity>();

            transform.ForEachDescendant(trans => {
                var slot = trans.GetComponent<LinkSlot>();
                if (slot != null)
                {
                    AddSlot(slot.SlotName, slot);
                }
            });
        }

        public bool AddSlot(string slotName, LinkSlot slot)
        {
            if (!Slots.TryGetValue(slotName, out LinkSlot old))
            {
                Debug.LogError("slotName已存在");
                throw new ArgumentException("slotName已存在");
            }
            else
            {
                Slots.Add(slotName, slot);
                slot.RootNid = NidComp.netId;
                return true;
            }
        }

        public LinkSlot GetSlot(string slotName)
        {
            Slots.TryGetValue(slotName, out LinkSlot result);
            return result;
        }

        public Transform GetTransform(string slotName)
        {
            return GetSlot(slotName)?.transform;
        }
    }
}
