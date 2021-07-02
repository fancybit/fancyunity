using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    [ExecuteInEditMode]
    public class SpriteDrawer2D : SerializedMonoBehaviour
    {
        private SpriteRenderer _renderer;
        private SpriteMask _sprMask;
        private Texture2D _tex;
        private Sprite _spr;
        private bool _inited;

        private void Awake()
        {
            if (!_inited) Initialize();
        }

        private void Start()
        {
            if (!_inited) Initialize();
        }

        [Button]
        void Initialize()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null)
            {
                _sprMask = GetComponent<SpriteMask>();
                if (_sprMask == null)
                {
                    Debug.LogError("没有Sprite渲染器也没有mask");
                    return;
                }
                else
                {
                    _sprMask.sprite = _spr;
                }
            }
            else
            {
                _spr = _renderer.sprite;
            }

            if (_spr == null)
            {
                _spr = _sprMask.sprite;
                _tex = new Texture2D(512, 512);
                _spr = Sprite.Create(_tex, new Rect(0, 0, 512, 512), Vector2.zero);
            }
            if (_renderer == null)
            {
                _sprMask.sprite = _spr;
            }
            else
            {
                _renderer.sprite = _spr;
            }

            _inited = true;
        }

        
        [Button]
        public void DrawHalfRect()
        {
            _tex.FillRect(new Rect(0, 0, _tex.width, _tex.height),Color.white);
            _tex.FillRect(new Rect(0, 0, _tex.width, _tex.height / 2),new Color(0,0,0,0));
        }
    }
}
