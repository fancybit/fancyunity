using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public abstract class FloatArea:MonoBehaviour
    {
        
        protected Collider2D _area;

        protected void init()
        {
            _area = GetComponent<Collider2D>();
            _area.isTrigger = true;
        }

        public abstract Vector3 GetTargetPoint(Floatable fltb);

        public Vector3 GetRandPosInArea()
        {
            var rect = new Rect(_area.bounds.min.x, _area.bounds.min.y,
                    _area.bounds.size.x, _area.bounds.size.y);
            var p = new Vector3();
            do
            {
                p.x = Random.Range(rect.xMin, rect.xMax);
                p.y = Random.Range(rect.yMin, rect.yMax);
            } while (!_area.bounds.Contains(p));
            return p;
        }
    }
}
