using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class FMath
    {
        public static Vector3 RayPlaneProj(Vector3 point, Vector3 direct,
            Vector3 planePoint, Vector3 planeNormal)
        {
            var distance = planePoint - point;
            float d = Vector3.Dot(distance, planeNormal) / Vector3.Dot(direct, planeNormal);
            direct *= d;
            return direct + point;
        }

        public static Vector3 RayPlaneProj(Ray ray,Vector3 planePoint, Vector3 planeNormal)
        {
            return RayPlaneProj(ray.origin, ray.direction,planePoint,planeNormal);
        }
    }
}
