using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FancyUnity
{
    public class FireSprite : MonoBehaviour
    {
        public Texture2D DisturbTex;
        public float Speed = 0.1f;
        public Vector2 Direction = Vector2.up;
        [Range(0f, 1f)]
        public float DisturbOffsetScale = 0.001f;
        public Vector2 DisturbSizeScale = new Vector2(10f, 10f);

        private Material _mat;
        private Vector2 _offset;

        void Start()
        {
            var oldMat = GetComponent<Renderer>().material;
            _mat = new Material(Shader.Find("FancyBit/2D/Fire-Sprite-Unlit"));
            var sprRenderer = GetComponent<SpriteRenderer>();
            sprRenderer.material = _mat;
            Destroy(oldMat);

            if (DisturbTex == null)
            {
                DisturbTex = Resources.Load<Texture2D>("FancyUnity/Images/扰动图");
            }
            _mat.SetTexture("_DisturbTex", DisturbTex);
            var spr = sprRenderer.sprite;
            var rect = spr.rect;
            rect.x /= spr.texture.width;
            rect.y /= spr.texture.height;
            rect.width /= spr.texture.width;
            rect.height /= spr.texture.height;
            _mat.SetVector("_UVRect", new Vector4(rect.x, rect.y, rect.width, rect.height));

            Direction.Normalize();
        }

        // Update is called once per frame
        void Update()
        {
            _offset -= Speed * Direction * Time.deltaTime;
            _mat.SetVector("_DisturbOffset", _offset);
            _mat.SetFloat("_DisturbOffsetScale", DisturbOffsetScale);
            _mat.SetVector("_DisturbSizeScale", DisturbSizeScale);
        }
    }
}