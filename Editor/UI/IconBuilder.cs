using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace FancyUnity
{
    public class IconBuilder : EditorWindow
    {
        public string ResultPath = "QXSOffice/Resources/Items/Icons/";
        public GameObject Template;
        public Sprite[] IconImages;

        [MenuItem("FancyUnity/批量生成图标")]
        public static void ShowWindow()
        {
            GetWindow(typeof(IconBuilder));
        }

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
            Debug.Log($"����{count}��ͼ��Ԥ�Ƽ�");
        }

        public virtual void OnPrefabCreated(GameObject prefab)
        {

        }
    }
}