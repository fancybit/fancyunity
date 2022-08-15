using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public static class ColorEffects
    {
        public static void StartShine(this Renderer self)
        {
            self.StopShine();
            self.material.DOFade(0f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }

        public static void StopShine(this Renderer self)
        {
            var mat = self.material;
            var c = mat.color;
            c.a = 1f;
            mat.color = c;
            //mat.DOComplete(true);
            mat.DOKill(true);
        }
    }
}
