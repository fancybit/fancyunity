using Mirror;
using SII = Sirenix.OdinInspector.ShowInInspectorAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class LinkSlot:MonoBehaviour
    {
        public string SlotName;
        [SII]
        public uint RootNid => transform.parent.GetComponentInParent<NetworkIdentity>().netId;
    }
}
