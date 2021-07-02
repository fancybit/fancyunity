using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class Delay
    {
        public static IEnumerator NextFrame(Action act)
        {
            yield return new WaitForEndOfFrame();
            act?.Invoke();
        }

        public static IEnumerator WaitForSeconds(Action act, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            act?.Invoke();
        }
    }
}
