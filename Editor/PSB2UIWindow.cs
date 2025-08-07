using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FancyUnity
{
    [EditorWindowTitle(title = "PSB to UGUI", useTypeNameAsIconName = false)]
    public class PSB2UIWindow : EditorWindow
    {
        private GameObject canvas;
        private float scale = 100;
        private bool bRecusivePercentMode = true;

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
                        if (spr.bounds.size.x == 0) scale = 100;
                        else scale = spr.rect.width / spr.bounds.size.x * 2;
                    }

                    //scale默认为100
                    //创建UI Canvas和必备组件
                    canvas = new GameObject();
                    canvas.name = "UICanvas";
                    var rtCanvas = canvas.AddComponent<RectTransform>();
                    rtCanvas.LocalReset();
                    var uiCanvas = canvas.AddComponent<Canvas>();
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
                    var rt = uiElem.AddComponent<RectTransform>();
                    map.Add(tr, rt);
                    //锚记放在中心
                    var anchorPos = new Vector2(0.5f, 0.5f);
                    rt.anchorMin = anchorPos;
                    rt.anchorMax = anchorPos;

                    //设置UI树节点RectTransform宽高
                    var bounds = tr.GetTotalBounds();
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        Mathf.Abs(bounds.size.x) * scale);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        Mathf.Abs(bounds.size.y) * scale);
                });

                //设置位置
                sel.transform.BFS(tr =>
                {
                    var uiElem = map[tr].gameObject;
                    //配置ui的层级关系
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
                    uiElem.transform.LocalReset();//所有图层和图层组在坐标原点

                    //位置
                    var rtTrans = uiElem.GetComponent<RectTransform>();
                    var transCanvas = canvas.transform as RectTransform;
                    var rtCanvas = canvas.transform as RectTransform;

                    var renderer = tr.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {//一般图层
                        var image = uiElem.gameObject.AddComponent<Image>();
                        image.sprite = renderer.sprite;
                        var pos = tr.transform.localPosition * scale;
                        //图层下移，因为ps的坐标系中心在底部中心
                        pos.y -= transCanvas.sizeDelta.y * 0.5f;
                        var pos2 = new Vector2(pos.x, pos.y);
                        rtTrans.anchoredPosition += pos2;
                    }
                });

                //移动图层组坐标 从原点到包含层图包围盒中心
                sel.transform.BFS(tr =>
                {
                    var rtTrans = map[tr];
                    var renderer = tr.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                    {//图层组
                        var bounds = tr.GetTotalBounds();
                        Vector2 diff = bounds.center * scale;
                        diff.y -= canvas.GetComponent<RectTransform>().sizeDelta.y*0.5f;

                        rtTrans.anchoredPosition += diff;
                        rtTrans.ForEach((rt, i) =>
                        {
                            rt.anchoredPosition -= diff;
                            return true;
                        });
                    }
                });

                DestroyImmediate(sel);
                Debug.Log("转换完成");
            }
        }
    }
}
