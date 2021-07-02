using FancyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FancyUnity
{
    public class Tools
    {
        [MenuItem("FancyUnity/TransparentWindow")]
        public static void Transparent()
        {
            WinAPI.TransWindow();
        }

        [MenuItem("FancyUnity/GameWindow")]
        public static void Game()
        {
            WinAPI.GameWindow();
        }

        [MenuItem("FancyUnity/NormalWindow")]
        public static void Normal()
        {
            WinAPI.NormalWindow();
        }
    }
}
