using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace FancyUnity
{
    public class RandFloatArea : FloatArea
    {
        public float MinTargetChangeInterval = 1f;
        public float MaxTargetChangeInterval = 3f;

        private Dictionary<Floatable, RandFloatData> _pointData = new Dictionary<Floatable, RandFloatData>();
        
        private void Start()
        {
            init();
        }

        public override Vector3 GetTargetPoint(Floatable fltb)
        {
            if(!_pointData.TryGetValue(fltb,out RandFloatData data))
            {
                data = new RandFloatData();
                _pointData.Add(fltb, data);
            }
            
            if (data.RemainTimeToChange <= 0f)
            {
                data.RemainTimeToChange = Random.Range(MinTargetChangeInterval, MaxTargetChangeInterval);
                data.CurPos = GetRandPosInArea();
            }
            return data.CurPos;
        }

        void Update()
        {
            foreach (var data in _pointData)
            {
                if (data.Value!=null && data.Value.RemainTimeToChange >= 0)
                {
                    data.Value.RemainTimeToChange -= Time.deltaTime;
                }
            }
        }

        public class RandFloatData
        {
            public Vector3 CurPos;
            public float RemainTimeToChange;
        }
    }
}
