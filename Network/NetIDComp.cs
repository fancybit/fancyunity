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
        public uint NetId;
        public void Update()
        {
            NetId = GetComponent<Mirror.NetworkIdentity>().netId;
        }
    }
}
