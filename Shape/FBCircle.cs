using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FancyUnity
{
    public class FBCircle : MonoBehaviour
    {
        public Material DrawMaterial;

        public int _segments = 3;
        public float _radius = 1f;
        public Color _color = Color.green;

        protected virtual void Start()
        {
            DrawMaterial = ResMgr.Inst.CreateRes<Material>("Materials/CircleMat");
        }

        [ShowInInspector]
        public Color MainColor
        {
            get => _color;
            set
            {
                DrawMaterial.color = value;
                _color = value;
            }
        }

        [ShowInInspector]
        public int Segments
        {
            get => _segments;
            set
            {
                if (value < 3) value = 3;
                _segments = value;
            }
        }

        [ShowInInspector]
        public float Radius
        {
            get => _radius;
            set
            {
                if (value < 0) value = 0;
                _radius = value;
            }
        }

        protected List<Vector3> arrVec = new List<Vector3>();

        protected virtual void OnRenderObject()
        {
            if (arrVec.Count == 0)
            {
                for (int i = 1; i <= Segments + 1; ++i)
                {
                    float angle = i * 2 * Mathf.PI / Segments;
                    arrVec.Add(new Vector3(Mathf.Cos(angle) * Radius, 
                        Mathf.Sin(angle) * Radius, 0));
                }
            }

            DrawMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(MainColor);
            for (int i = 0; i < arrVec.Count-1; i++)
            {
                GL.Vertex(arrVec[i]);
                GL.Vertex(arrVec[i+1]);
            }
            GL.Vertex(arrVec[0]);
            GL.End();
            GL.PopMatrix();
            GL.Flush();
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
