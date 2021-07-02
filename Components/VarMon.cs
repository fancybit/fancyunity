using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FancyUnity
{
    public class VarMon : SerializedMonoBehaviour
    {
        private static Dictionary<string, string> _fields = new Dictionary<string, string>();
        void OnGUI()
        {
            lock (_fields)
            {
                foreach (var f in _fields)
                {
                    GUILayout.TextField($"{f.Key}={f.Value}");
                }
            }
        }

        public static void Log(string valName, object value)
        {
            lock (_fields)
            {
                _fields[valName] = value.ToString();
            }
        }
    }
}
