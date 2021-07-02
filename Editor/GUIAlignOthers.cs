using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FancyCSharp;

namespace FancyUnity
{
    /// <summary>  
    /// Introduction: GUIAlignOthers  
    /// Author:     Cheng  
    /// Time:   
    /// </summary>  
    public class GUIAlignOthers : Editor
    {
        [MenuItem("GameObject/UI/Align/Top")]
        static void AlignTop()
        {
            Align(1);
        }
        [MenuItem("GameObject/UI/Align/Left")]
        static void AlignLeft()
        {
            Align(2);
        }
        [MenuItem("GameObject/UI/Align/Right")]
        static void AlignRight()
        {
            Align(3);
        }
        [MenuItem("GameObject/UI/Align/Bottom")]
        static void AlignBottom()
        {
            Align(4);
        }
        [MenuItem("GameObject/UI/Align/Horizontal")]
        static void AlignHorizontal()
        {
            Align(5);
        }
        [MenuItem("GameObject/UI/Align/Vertical")]
        static void AlignVertical()
        {
            Align(6);
        }
        static void Align(int type)
        {
            List<RectTransform> rects = new List<RectTransform>();
            GameObject[] objects = Selection.gameObjects;
            foreach (var obj in objects)
            {
                RectTransform rect = obj.GetComponent<RectTransform>();
                if (rect != null)
                    rects.Add(rect);
            }
            if (rects.Count > 1)
            {
                Align(type, rects);
            }
        }
        static void Align(int type, List<RectTransform> rects)
        {
            RectTransform tenplate = rects[0];
            float w = tenplate.sizeDelta.x * tenplate.lossyScale.x;//消除缩放影响  
            float h = tenplate.sizeDelta.y * tenplate.localScale.y;
            float x = tenplate.position.x - tenplate.pivot.x * w; //消除中心点并非pivot非（0.5，0.5）影响  
            float y = tenplate.position.y - tenplate.pivot.y * h;
            switch (type)
            {
                case 1://上对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float th = trans.sizeDelta.y * trans.localScale.y;
                        Vector3 pos = trans.position;
                        pos.y = y + h - th + trans.pivot.y * th;
                        trans.position = pos;
                    }
                    break;
                case 2://左对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float tw = trans.sizeDelta.x * trans.lossyScale.x;
                        Vector3 pos = trans.position;
                        pos.x = x + tw * trans.pivot.x;
                        trans.position = pos;
                    }
                    break;
                case 3://右对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float tw = trans.sizeDelta.x * trans.lossyScale.x;
                        Vector3 pos = trans.position;
                        pos.x = x + w - tw + tw * trans.pivot.x;
                        trans.position = pos;
                    }
                    break;
                case 4://下对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float th = trans.sizeDelta.y * trans.localScale.y;
                        Vector3 pos = trans.position;
                        pos.y = y + th * trans.pivot.y;
                        trans.position = pos;
                    }
                    break;
                case 5://水平对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float th = trans.sizeDelta.y * trans.localScale.y;
                        Vector3 pos = trans.position;
                        pos.y = y + 0.5f * h - 0.5f * th + th * trans.pivot.y;
                        trans.position = pos;
                    }
                    break;
                case 6://垂直对齐  
                    for (int i = 1; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float tw = trans.sizeDelta.x * trans.lossyScale.x;
                        Vector3 pos = trans.position;
                        pos.x = x + 0.5f * w - 0.5f * tw + tw * trans.pivot.x;
                        trans.position = pos;
                    }
                    break;
            }
        }
    }
}