using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FancyCSharp;

namespace FancyUnity
{
    public class StringExtTest : MonoBehaviour
    {
        private void Start()
        {
            var str1 = "1234";
            var str2 = "abcd";

            Debug.Log(str1.Slice(0, -1));
            Debug.Log(str2.Slice(-2, -1));
            Debug.Log("q123".SubStringEx(-3,2));
        }
    }
}
