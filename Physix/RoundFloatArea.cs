using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class RoundFloatArea : FloatArea
    {
        public float MinRadius = 0.5f;
        public float MaxRadius = 3f;
        public float AngleSpeed = 180f;
        public float MinCenterSpeed = 1f;
        public float MaxCenterSpeed = 2f;

        private Dictionary<Floatable, RoundFloatData> _pointData = new Dictionary<Floatable, RoundFloatData>();

        private void Start()
        {
            init();
        }

        private void resetData(ref RoundFloatData data)
        {
            data.Center = data.OriginPos;
            data.CenterSpeed = Random.Range(MinCenterSpeed, MaxCenterSpeed);
            data.Radius = Random.Range(MinRadius, MaxRadius);
            data.Angle = Random.Range(0, 360);
        }

        public override Vector3 GetTargetPoint(Floatable fltb)
        {
            Vector3 ret;
            if (!_pointData.TryGetValue(fltb, out RoundFloatData data))
            {
                data = new RoundFloatData();
                data.OriginPos = fltb.transform.position;
                resetData(ref data);
                _pointData.Add(fltb, data);
            }
            ret = data.GetPos();
            return ret;
        }

        void Update()
        {
            foreach (var pair in _pointData)
            {
                var data = pair.Value;
                data.Angle += AngleSpeed * Time.deltaTime;
                data.Center.x += data.CenterSpeed * Time.deltaTime;
                if (!_area.bounds.Contains(data.Center))
                {
                    resetData(ref data);
                }
            }
        }

        private Vector2 GetNewCenterPosInArea()
        {
            var rect = new Rect(_area.bounds.min.x, _area.bounds.min.y,
                _area.bounds.size.x, _area.bounds.size.y);
            var p = new Vector3();
            do
            {
                p.x = rect.xMin;
                p.y = Random.Range(rect.yMin, rect.yMax)*0.8f;
            } while (!_area.bounds.Contains(p));
            return p;
        }
    }

    public class RoundFloatData
    {
        public Vector2 Center;
        public Vector2 OriginPos;
        public float CenterSpeed;
        public float Radius;
        public float Angle;
        public Vector2 GetPos()
        {
            var ret = Center;
            ret.x += Mathf.Cos(Angle * Mathf.Deg2Rad) * Radius;
            ret.y += Mathf.Sin(Angle * Mathf.Deg2Rad) * Radius;
            return ret;
        }
    }
}
