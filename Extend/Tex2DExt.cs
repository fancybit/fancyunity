using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public static class Tex2DExt
    {
        public static void FillCircle(this Texture2D self, Vector2 center, float radius = -1, Color? color = null, float ratio = 1)
        {
            self.FillCircle(center.x, center.y, radius, color, ratio);
        }

        public static void FillCircle(this Texture2D self, float cx, float cy, float radius = -1, Color? color = null, float ratio = 1)
        {
            var _color = color ?? Color.red;
            if (radius < 0)
            {
                var minLength = Mathf.Min(self.width, self.height);
                radius = minLength * 0.5f;
            }
            for (var x = cx - radius; x < cx + radius; ++x)
            {
                for (var y = cy - radius * ratio; y < cy + radius * ratio; ++y)
                {
                    if (Mathf.Pow(x - cx, 2) + Mathf.Pow((y - cy) / ratio, 2) < radius * radius)
                        self.SetPixel((int)x, (int)y, _color);
                }
            }
            self.Apply();
        }

        public static void FillRect(this Texture2D self, Rect rect, Color? c = null)
        //,float ratio = 1)
        //TODO:完善自动修正比例变化
        {
            if (c == null) c = Color.red;
            for (var x = rect.xMin; x < rect.xMax; ++x)
            {
                for (var y = rect.yMin; y < rect.yMax; ++y)
                {
                    self.SetPixel((int)(x), (int)(y), c.Value);
                }
            }
            self.Apply();
        }

        public static void Clear(this Texture2D self, Color? c = null)
        {
            var rect = new Rect(0, 0, self.width, self.height);
            self.FillRect(rect, c);
        }

        public static float CountSolidPixels(this Texture2D self)
        {
            var pixels1 = self.GetPixels(0, 0, self.width, self.height);
            var solidPixelCount = 0f;
            foreach (var pixel in pixels1)
            {
                solidPixelCount += pixel.a;
            }
            return solidPixelCount;
        }

        public static Texture2D Clone(this Texture2D self)
        {
            var ret = new Texture2D(self.width, self.height);
            ret.SetPixels(0, 0, self.width, self.height, self.GetPixels());
            return ret;
        }

        public static Texture2D AlphaIntersect(Texture2D texSymbol, Texture2D texMask, float threshold = 0.8f)
        {
            if (texSymbol.width != texMask.width || texSymbol.height != texMask.height)
            {
                Debug.LogError("贴图尺寸不同");
                return null;
            }
            var texRet = new Texture2D(texMask.width, texMask.height);
            var color = new Color(0, 0, 0, 0);
            texRet.Clear(color);
            for (var x = 0; x < texMask.width; ++x)
            {
                for (var y = 0; y < texMask.height; ++y)
                {
                    var c1 = texSymbol.GetPixel(x, y);
                    var c2 = texMask.GetPixel(x, y);
                    var a = (c1.a + c2.a) * 0.5f;
                    if (a > threshold)
                    {
                        color.r = a;
                        color.g = a;
                        color.b = a;
                        color.a = a;
                        texRet.SetPixel(x, y, color);
                    }
                }
            }
            texRet.Apply();
            return texRet;
        }
    }
}