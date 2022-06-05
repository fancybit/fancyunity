using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FancyUnity
{
    [ExecuteInEditMode]
    public class FBCircle : MonoBehaviour
    {
        public Material DrawMaterial;

        [SerializeField]
        protected int _segments = 3;
        [SerializeField]
        protected float _radius = 1f;
        [SerializeField]
        protected Color _color = Color.green;

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

        private void OnRenderObject()
        {
            DrawMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            arrVec.Clear();
            for (int i = 1; i <= Segments+1; ++i)
            {
                float angle = i * 2 * Mathf.PI / Segments;
                arrVec.Add(new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0));
            }
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

    }
}
