using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class Floatable : MonoBehaviour
    {
        //public float Force = 1f;
        public float Speed = 0.5f;

        //private Vector3 Accelerator;
        //private Vector3 Velocty;
        private Transform Target;
        public FloatArea _floatArea;

        private void Start()
        {
            GetComponent<Collider2D>().isTrigger = true;
            var goTarget = new GameObject($"{name}'s Target");
            Target = goTarget.transform;
            Target.SetParent(transform.parent);
            Target.localPosition = Vector3.zero;
        }

        private void Update()
        {
            /*
            var pos = transform.position;
            var targetPos = Target.position;
            var dir = targetPos - pos;
            var dist = dir.magnitude;*/

            if (_floatArea != null)
            {//更新位置
                Target.position = _floatArea.GetTargetPoint(this);
            }

            var pos = transform.position;
            var targetPos = Target.position;
            transform.position = Vector3.Lerp(pos, targetPos, Mathf.Min(Speed * Time.deltaTime));

            /*
            Accelerator = dir.normalized * Time.deltaTime * Force * Mathf.Atan(dist);
            Velocty += Accelerator;
            var velMag = Velocty.magnitude;
            if (velMag > MaxSpeed)
            {
                Velocty.Normalize();
                Velocty *= MaxSpeed;
            }
            pos += Velocty * Time.deltaTime;
            transform.position = pos;*/
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(_floatArea==null) _floatArea = collision.GetComponent<FloatArea>();
        }
    }
}
