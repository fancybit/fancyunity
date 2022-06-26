using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace FancyUnity
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class NetIDComp : MonoBehaviour
    {
        [ShowInInspector]
        public uint NetID => GetComponent<Mirror.NetworkIdentity>().netId;
    }
}
