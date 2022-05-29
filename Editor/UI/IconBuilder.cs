using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

namespace FancyUnity
{
    public class IconBuilder : OdinEditorWindow
    {
        public string ResultPath = "QXSOffice/Resources/Items/Icons/";
        public GameObject Template;
        public Sprite[] IconImages;

        [MenuItem("FancyUnity/批量生成图标")]
        public static void ShowWindow()
        {
            GetWindow(typeof(IconBuilder));
        }

        [Button(name: "生成")]
        public void Generate()
        {
            int count = 0;
            var proto = GameObject.Instantiate(Template);
            for (var i = 0; i < IconImages.Length; ++i)
            {
                var img = IconImages[i];
                var imgComp = proto.transform.Find("MaskLayer/Content").GetComponent<Image>();
                imgComp.sprite = img;
                var path = $"{ResultPath}{img.name}.prefab";
                var prefab = PrefabUtility.SaveAsPrefabAsset(proto, $"{Application.dataPath}/{path}");
                OnPrefabCreated(prefab);
                ++count;
            }
            DestroyImmediate(proto);
            Debug.Log($"生成{count}个图标预制件");
        }

        public virtual void OnPrefabCreated(GameObject prefab)
        {

        }
    }
}