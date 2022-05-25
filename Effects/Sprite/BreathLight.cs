using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyUnity
{
    public class BreathLight : MonoBehaviour
    {
        public float Max = 1f;
        public float Min = 0f;
        public float Interval = 1f;

        private SpriteRenderer _renderer;
        private Material _mat;
        private float _originAlpha;

        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _mat = _renderer.material;
            _originAlpha = _renderer.color.a;
        }

        // Update is called once per frame
        void Update()
        {
            var s = Mathf.PingPong(Time.time, Interval) * (Max - Min) + Min;
            var c = _renderer.color;
            c.a = _originAlpha * s;
            _renderer.color = c;
        }
    }
}