using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FancyUnity
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AlphaMixTest : SerializedMonoBehaviour
    {
        public SpriteRenderer Spr1;
        public SpriteRenderer Spr2;

        [Range(0f, 1f)]
        public float Alpha = 0.5f;
        [Range(0f, 1f)]
        public float Threashold = 0.8f;

        private Texture2D _tex1;
        private Texture2D _tex2;
        private SpriteRenderer _renderer;

        // Start is called before the first frame update
        void Start()
        {
            _tex1 = Spr1.sprite.texture;
            _tex2 = Spr2.sprite.texture;
            _renderer = GetComponent<SpriteRenderer>();
        }

        [Button]
        void Intersect()
        {
            var tex = Tex2DExt.AlphaIntersect(_tex1, _tex2, Threashold);
            _renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 100);
        }

        void Update()
        {
            var color = Color.white;
            color.a = Alpha;
            Spr1.color = color;
            Spr2.color = color;
        }
    }
}