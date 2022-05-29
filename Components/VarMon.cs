using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FancyUnity
{
    public class VarMon : Singleton<VarMon>
    {
        private Dictionary<string, string> _fields = new Dictionary<string, string>();
        public GUIStyle Style = new GUIStyle();

        void OnGUI()
        {
            lock (_fields)
            {
                GUILayout.BeginVertical(Style);
                foreach (var f in _fields)
                {
                    GUILayout.TextField($"{f.Key}={f.Value}",Style);
                }
                GUILayout.EndVertical();
            }
        }

        public void Log(string valName, object value)
        {
            lock (_fields)
            {
                _fields[valName] = value.ToString();
            }
        }
    }
}
