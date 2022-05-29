using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FancyUnity
{
    public static class RenderExt
    {
        public static void SetGray(this Image self, float value)
        {
            
        }

        public static void ShowTreeSprite(this Transform self, float t=1f)
        {
            self.ForEachDescendant(trans => {
                DOTween.Complete(self.gameObject);
                var sr = self.GetComponent<SpriteRenderer>().DOFade(1f,t);
            });
        }

        public static void ShowTreeUI(this Transform self, float t=1f)
        {
            self.ForEachDescendant(trans => {
                DOTween.Complete(self.gameObject);
                var sr = self.GetComponent<Image>().DOFade(1f, t);
            });
        }

        public static void HideTreeSprite(this Transform self, float t=1f)
        {
            self.ForEachDescendant(trans => {
                DOTween.Complete(self.gameObject);
                var sr = self.GetComponent<SpriteRenderer>().DOFade(0f, t);
            });
        }

        public static void HideTreeUI(this Transform self, float t = 1f)
        {
            self.ForEachDescendant(trans => {
                DOTween.Complete(self.gameObject);
                var sr = self.GetComponent<Image>().DOFade(0f, t);
            });
        }

    }

}
