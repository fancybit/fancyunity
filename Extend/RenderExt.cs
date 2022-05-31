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

        public static void ShowTreeSprite(this Transform self,  float t=1f, Sequence seq=null)
        {
            self.ForEachDescendant(trans => {
                var sr = trans.GetComponent<SpriteRenderer>();
                sr.color = new Color(1, 1, 1, 0);
                var anim = sr.DOFade(1f, t);
                if (seq != null) seq.Join(anim);
                else anim.Play();
            });
        }

        public static void ShowTreeUI(this Transform self, float t = 1f,Sequence seq=null)
        {
            self.ForEachDescendant(trans => {
                var sr = trans.GetComponent<Image>();
                sr.color = new Color(1, 1, 1, 0);
                var anim = sr.DOFade(1f, t);
                if (seq != null) seq.Join(anim);
                else anim.Play();
            });
        }

        public static void HideTreeSprite(this Transform self, float t=1f,Sequence seq = null)
        {
            self.ForEachDescendant(trans => {
                var sr = trans.GetComponent<SpriteRenderer>();
                sr.color = new Color(1, 1, 1, 1);
                var anim = sr.DOFade(0f, t);
                if (seq != null) seq.Join(anim);
                else anim.Play();
            });
        }

        public static void HideTreeUI(this Transform self,  float t = 1f,Sequence seq=null)
        {
            self.ForEachDescendant(trans => {
                var sr = trans.GetComponent<Image>();
                sr.color = new Color(1, 1, 1, 1);
                var anim = sr.DOFade(0f, t);
                if(seq!=null) seq.Join(anim);
                else anim.Play();
            });
        }
    }
}
