using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FancyCSharp;
using System.IO;
using System.Text.RegularExpressions;

namespace FancyUnity
{

    public class BatcherWindow : EditorWindow
    {
        private string layerName;

        private string nameSpace;
        private Regex regex0 = new Regex(@"namespace\s+(\w+);\s*",
            RegexOptions.Compiled |
            RegexOptions.Multiline);
        private Regex regex1 = new Regex(@"using\s+(\w)+\s+.+public\s+class",
            RegexOptions.Compiled |
            RegexOptions.Multiline);


        [MenuItem("FancyUnity/批量操作")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BatcherWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("批量替换图层");
            GUILayout.Label("图层名：");
            layerName = GUILayout.TextField(layerName);
            if (GUILayout.Button("批量改Layer"))
                foreach (var obj in Selection.objects)
                {
                    var trans = (obj as GameObject)?.transform;
                    if (trans == null) continue;
                    trans.ForEachDescendant((tr) =>
                        tr.gameObject.layer = LayerMask.NameToLayer(layerName));
                }

            GUILayout.Label("批量更换命名空间");
            GUILayout.Label("新命名空间");
            nameSpace = GUILayout.TextField(nameSpace);
            if (GUILayout.Button("替换"))
            {
                int count = 0;
                foreach (var obj in Selection.objects)
                {
                    if (!(obj is MonoBehaviour)) continue;
                    var filePath = AssetDatabase.GetAssetPath(obj);
                    string code;
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (var sr = new StreamReader(fs))
                    {
                        code = sr.ReadToEnd();
                    };

                    var matches0 = regex0.Match(code);
                    var matches1 = regex1.Match(code);
                    if (matches0.Success)
                    {
                        code = regex0.Replace(code, $"namespace {nameSpace};\n");
                    }
                    else if (matches1.Success)
                    {
                        code = regex1.Replace(code, $"using {nameSpace};\npublic class");
                    }

                    File.Delete(filePath);
                    using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(code);
                    };
                    ++count;
                }
                Debug.Log($"替换了{count}个脚本文件的命名空间.");
            }
        }
    }
}

