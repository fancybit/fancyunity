using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FancyUnity
{
    public class FBCircle : NetworkBehaviour
    {
        public Material DrawMaterial;

        public int Segments = 128;
        [SyncVar]
        public float Radius = 1f;
        public Color Color = Color.green;

        protected virtual void Start()
        {
            DrawMaterial = ResMgr.Inst.CreateRes<Material>("Materials/CircleMat");
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
            GL.Color(Color);
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
