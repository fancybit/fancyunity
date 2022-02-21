using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FancyCSharp;

namespace FancyUnity
{
    public class PSB2UIWindow : EditorWindow
    {
        private GameObject canvas;
        private RectTransform uiTrans;
        private Canvas uiCanvas;
        private float scale = 100;

        [MenuItem("FancyUnity/PSB转UGUI")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PSB2UIWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("PSB预制件转UGUI");
            GUILayout.Label($"缩放比例：{scale}");
            GUILayout.Label("预制件名：");
            GUILayout.TextField(Selection.activeGameObject?.name ?? "未选择");
            if (GUILayout.Button("创建Canvas"))
            {
                //获取当前选择的预制件
                var sel = Selection.activeGameObject;
                if (sel != null)
                {
                    //获取预制件psb文件的对应缩放比例
                    var img = sel.transform.FindFirstChild(tr =>
                    {//找到第一个实际的图层（sprite）
                        var sr = tr.GetComponent<SpriteRenderer>();
                        return sr != null && sr.sprite != null;
                    });

                    if (img != null)
                    {//计算缩放比例
                        var spr = img.GetComponent<SpriteRenderer>().sprite;
                        scale = spr.rect.width / spr.bounds.size.x * 2;
                    }

                    //scale默认为100
                    //创建UI Canvas和必备组件
                    canvas = new GameObject();
                    canvas.name = "UICanvas";
                    uiTrans = canvas.AddComponent<RectTransform>();
                    uiTrans.LocalReset();
                    uiCanvas = canvas.AddComponent<Canvas>();
                    uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    uiCanvas.pixelPerfect = true;
                    canvas.AddComponent<CanvasScaler>();
                    canvas.AddComponent<GraphicRaycaster>();
                    var evtSystem = new GameObject("EventSystem");
                    evtSystem.AddComponent<RectTransform>();
                    evtSystem.transform.SetParent(canvas.transform);
                    evtSystem.transform.LocalReset();
                    evtSystem.AddComponent<EventSystem>();
                    evtSystem.AddComponent<StandaloneInputModule>();
                }
            }
            if (GUILayout.Button("开始转换"))
            {
                var sel = Selection.activeGameObject;
                if (sel == null)
                {
                    Debug.Log("未选择源预制件");
                    return;
                }
                //制作副本按照SpriteRenderer的数值排序图层
                var selName = sel.name;
                sel = Instantiate(sel);
                sel.name = selName;
                var lst = new List<Transform>();
                sel.transform.ForEachDescendant(tr => lst.Add(tr));
                foreach (var tr in lst)
                {
                    tr.SortChildren(tr =>
                    {
                        var weight = 0;
                        tr.DFS(tr =>
                        {
                            var sr = tr.GetComponent<SpriteRenderer>();
                            if (sr != null)
                            {
                                weight = sr.sortingOrder;
                                return false;
                            }
                            return true;
                        });
                        return weight;
                    });
                };

                //创建和SpriteRenderer渲染的2D图层结构同构的UI树形结构
                //对应关系记录入一个map
                var map = new Dictionary<Transform, RectTransform>();
                sel.transform.ForEachDescendant(tr =>
                {
                    var uiElem = new GameObject(tr.name);
                    map.Add(tr, uiElem.AddComponent<RectTransform>());
                });

                //配置ui的层级关系 添加图层组件
                sel.transform.BFS(tr =>
                {
                    var uiElem = map[tr].gameObject;
                    if (tr.parent != null &&
                        map.TryGetValue(tr.parent, out RectTransform uiElemParent) &&
                        uiElemParent != null)
                    {
                        uiElem.transform.SetParent(uiElemParent);
                    }
                    else
                    {
                        uiElem.transform.SetParent(canvas.transform);
                    }
                    uiElem.transform.LocalReset();

                    //位置
                    uiTrans = uiElem.GetComponent<RectTransform>();
                    uiTrans.anchorMin = new Vector2(0.5f, 0.5f);
                    uiTrans.anchorMax = new Vector2(0.5f, 0.5f);
                    uiTrans.offsetMin = uiTrans.offsetMin = Vector2.zero;
                    uiTrans.offsetMax = uiTrans.offsetMax = Vector2.zero;

                    //图片
                    var renderer = tr.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {//一般图层
                        uiTrans.offsetMax = uiTrans.offsetMin
                            = tr.transform.localPosition * scale;

                        var image = uiElem.gameObject.AddComponent<Image>();
                        image.sprite = renderer.sprite;
                        image.SetNativeSize();
                    }
                    else
                    {//调整图层组中心点
                        var rtUIElem = map[tr] as RectTransform;
                        //图层组以包含的所有图层的包围框几何中心为中心
                        var center = rtUIElem.GetTotalRect().center;
                        var diff = center - rtUIElem.offsetMin;
                        uiTrans.offsetMin += diff;
                        uiTrans.offsetMax += diff;
                        rtUIElem.ForEach((rt, i) =>
                        {
                            rt.offsetMin -= diff;
                            rt.offsetMax -= diff;
                            return true;
                        });
                    }
                });
                var rootRect = map[sel.transform];
                var centerPos = new Vector2(0.5f, 0f);
                rootRect.anchorMin = centerPos;
                rootRect.anchorMax = centerPos;
                rootRect.offsetMin = rootRect.offsetMin = Vector2.zero;
                DestroyImmediate(sel);
                Debug.Log("转换完成");
            }
        }
    }
}
