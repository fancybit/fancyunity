using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace FancyUnity
{
    public static class SpaceExt
    {
        public static Vector3 GetWorldMousePosition()
        {
            //获取屏幕坐标
            var mousePosition = Input.mousePosition;
            //屏幕坐标转世界坐标
            var worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            return worldPos;
        }

        public static Vector2 GetLocalMousePosition(this GameObject self)
        {
            //世界坐标转本地坐标
            var mouseLocalPos = self.transform.worldToLocalMatrix.MultiplyPoint(GetWorldMousePosition());
            mouseLocalPos.z = 0;
            return mouseLocalPos;
        }

        public static bool SendMessageEx<ParamType>(this GameObject self, string message, ParamType parm)
        {
            foreach (var comp in self.GetComponents<Component>())
            {
                var methodInfo = comp.GetType().GetMethod(message, new Type[] { typeof(ParamType) });
                Debug.Log($"comp name:{comp.GetType().Name} method name: {message}");
                if (methodInfo != null)
                {
                    Debug.Log($"founded");
                    var ret = (bool)methodInfo.Invoke(comp, new object[] { parm });
                    return ret;
                }
            }
            return false;
        }

        public static Transform FindFirstChild(this Transform self, Func<Transform, bool> predicate)
        {
            if (predicate(self)) return self;
            for (var i = 0; i < self.childCount; ++i)
            {
                var trans = self.GetChild(i);
                var ret = trans.FindFirstChild(predicate);
                if (ret != null) return ret;
            };
            return null;
        }

        public static Transform FindFirstChild(this Transform self, string name)
        {
            return self.FindFirstChild(trans => trans.name == name);
        }

        private static void doFindChildren(
            Transform self, List<Transform> lst, Func<Transform, bool> predicate)
        {
            if (predicate(self))
            {
                lst.Add(self);
            }
            self.ForEach((trans, idx) =>
            {
                doFindChildren(trans, lst, predicate);
                return true;
            });
        }

        public static List<Transform> FindChildren(this Transform self, Func<Transform, bool> predicate)
        {
            var lst = new List<Transform>();
            doFindChildren(self, lst, predicate);
            return lst;
        }


        public static Transform FindParent(this Transform self, Func<Transform, bool> predicate)
        {
            if (predicate(self)) return self;
            if (self.parent != null) return FindParent(self.parent, predicate);
            return null;
        }

        public static Transform FindParentByName(this Transform self, string name)
        {
            return self.FindParent(t => t.name == name);
        }

        public static T FindParentByComponent<T>(this Transform self)
        {
            return self.FindParent(t => t.GetComponent<T>() != null).GetComponent<T>();
        }

        public static (T, float) FindMax<T>(this IEnumerable<T> self, Func<T, float> visitor)
        {
            float curValue = float.MinValue;
            T curElem = default(T);
            foreach (var elem in self)
            {
                var v = visitor(elem);
                if (v > curValue)
                {
                    curValue = v;
                    curElem = elem;
                }
            }
            return (curElem, curValue);
        }

        public static (T, float) FindMin<T>(this IEnumerable<T> self, Func<T, float> visitor)
        {
            float curValue = float.MaxValue;
            T curElem = default(T);
            foreach (var elem in self)
            {
                var v = visitor(elem);
                if (v < curValue)
                {
                    curValue = v;
                    curElem = elem;
                }
            }
            return (curElem, curValue);
        }

        public static (Transform, float) FindMax(this Transform self, Func<Transform, float> visitor)
        {
            float curValue = float.MinValue;
            Transform curTrans = null;
            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                var v = visitor(child);
                if (v > curValue)
                {
                    curValue = v;
                    curTrans = child;
                }
            }
            return (curTrans, curValue);
        }

        public static (Transform, float) FindMin(this Transform self, Func<Transform, float> visitor)
        {
            float curValue = float.MaxValue;
            Transform curTrans = null;
            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                var v = visitor(child);
                if (v < curValue)
                {
                    curValue = v;
                    curTrans = child;
                }
            }
            return (curTrans, curValue);
        }

        public static Rect GetScreenSpaceRect(this RectTransform self)
        {
            Vector2 size = Vector2.Scale(self.rect.size, self.lossyScale);
            return new Rect((Vector2)self.position - (size * 0.5f), size);
        }

        public static void ForEach(this Transform self, Func<Transform, int, bool> visitor)
        {
            for (var i = 0; i < self.childCount; ++i)
            {
                var transChild = self.GetChild(i);
                if (!visitor(transChild, i)) return;
            }
        }

        public static void ForEachDescendant(this Transform self, Action<Transform> visitor)
        {
            visitor(self);
            self.ForEach((trans, _) =>
            {
                trans.ForEachDescendant(visitor);
                return true;
            });
        }

        public static void ClearChildren(this Transform self)
        {
            while (self.childCount > 0)
            {
                var o = self.GetChild(0);
                o.SetParent(null);
                GameObject.Destroy(o.gameObject);
            }
        }

        public static void SortChildren(this Transform self, Func<Transform, int> visitor)
        {
            var lst = new List<Transform>();
            self.ForEach((c, i) =>
            {
                lst.Add(c);
                return true;
            });
            lst.Sort((a, b) => visitor(a) - visitor(b));
            for (var i = 0; i < self.childCount; ++i)
            {
                lst[i].SetSiblingIndex(i);
            }
        }

        public static void SetPosX(this Transform self, float value)
        {
            var v3 = self.transform.position;
            v3.x = value;
            self.transform.position = v3;
        }
        public static void SetPosY(this Transform self, float value)
        {
            var v3 = self.transform.position;
            v3.y = value;
            self.transform.position = v3;
        }
        public static void SetPosZ(this Transform self, float value)
        {
            var v3 = self.transform.position;
            v3.z = value;
            self.transform.position = v3;
        }
        public static void SetLocalPosX(this Transform self, float value)
        {
            var v3 = self.transform.localPosition;
            v3.x = value;
            self.transform.localPosition = v3;
        }
        public static void SetLocalPosY(this Transform self, float value)
        {
            var v3 = self.transform.localPosition;
            v3.y = value;
            self.transform.localPosition = v3;
        }
        public static void SetLocalPosZ(this Transform self, float value)
        {
            var v3 = self.transform.localPosition;
            v3.z = value;
            self.transform.localPosition = v3;
        }

        public static void SetLocalScaleX(this Transform self, float value)
        {
            var v3 = self.transform.localScale;
            v3.x = value;
            self.transform.localScale = v3;
        }
        public static void SetLocalScaleY(this Transform self, float value)
        {
            var v3 = self.transform.localScale;
            v3.y = value;
            self.transform.localScale = v3;
        }
        public static void SetLocalScaleZ(this Transform self, float value)
        {
            var v3 = self.transform.localScale;
            v3.z = value;
            self.transform.localScale = v3;
        }
        public static void SetRotationX(this Transform self, float value)
        {
            var v3 = self.transform.eulerAngles;
            v3.x = value;
            self.transform.eulerAngles = v3;
        }
        public static void SetRotationY(this Transform self, float value)
        {
            var v3 = self.transform.eulerAngles;
            v3.y = value;
            self.transform.eulerAngles = v3;
        }
        public static void SetRotationZ(this Transform self, float value)
        {
            var v3 = self.transform.eulerAngles;
            v3.z = value;
            self.transform.eulerAngles = v3;
        }
        public static void SetLocalRotationX(this Transform self, float value)
        {
            var v3 = self.transform.localEulerAngles;
            v3.x = value;
            self.transform.localEulerAngles = v3;
        }
        public static void SetLocalRotationY(this Transform self, float value)
        {
            var v3 = self.transform.localEulerAngles;
            v3.y = value;
            self.transform.localEulerAngles = v3;
        }
        public static void SetLocalRotationZ(this Transform self, float value)
        {
            var v3 = self.transform.localEulerAngles;
            v3.z = value;
            self.transform.localEulerAngles = v3;
        }

        public static void LocalReset(this Transform self)
        {
            self.transform.localPosition = Vector3.zero;
            self.transform.localRotation = Quaternion.identity;
            self.transform.localScale = Vector3.one;
        }
    }
}