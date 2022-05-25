using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FancyUnity
{
    public class MaskRoll : MonoBehaviour
    {
        public float Speed = 0.1f;
        public Vector2 Direction = new Vector2(1, 1);

        private Material _mat;
        public Vector2 _offset;
        public Rect _rect;

        // Start is called before the first frame update
        void Start()
        {
            _mat = new Material(Shader.Find("FancyBit/2D/MaskRoll-Sprite"));
            var oldMat = GetComponent<Renderer>().material;
            var renderer = GetComponent<SpriteRenderer>();
            renderer.material = _mat;
            var spr = renderer.sprite;
            _rect = spr.rect;
            _rect.x /= spr.texture.width;
            _rect.y /= spr.texture.height;
            _rect.width /= spr.texture.width;
            _rect.height /= spr.texture.height;
            _mat.SetVector("_UVRect", new Vector4(_rect.x, _rect.y, _rect.width, _rect.height));
            Destroy(oldMat);
        }

        // Update is called once per frame
        void Update()
        {
            Direction.Normalize();
            _offset -= Speed * Direction * Time.deltaTime;
            _mat.SetVector("_Offset", _offset);
        }
    }
}