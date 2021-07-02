using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace FancyUnity
{
    public class FindMissingScriptsRecursively : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;

        [MenuItem("FancyUnity/丢失脚本处理")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public void OnGUI()
        {
            if (GUILayout.Button("查找选定目标丢失的脚本数量"))
            {
                FindInSelected();
            }
            if (GUILayout.Button("清理选定目标丢失的脚本"))
            {
                DestoryMissScript();
            }
        }

        public static void DestoryMissScript()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            var gos = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
            Selection.activeGameObject.transform.ForEachDescendant(trans =>
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(trans.gameObject);
            });
            AssetDatabase.Refresh();
            Debug.Log("清理完成!");
        }

        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }

        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):    
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );    
                FindInGO(childT.gameObject);
            }
        }
    }
}