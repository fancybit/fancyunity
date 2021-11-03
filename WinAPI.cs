using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public static class WinAPI
    {
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, uint wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, ulong dwNewLong);

        [DllImport("user32.dll")]
        public static extern ulong GetWindowLong(IntPtr hWnd, int nlndex);

        [DllImport("Dwmapi.dll")]
        public static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        [DllImport("user32.dll")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        #region DLL Imports


        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
        #endregion


        // Definitions of window styles
        public const ulong WS_OVERLAPPED = 0x00000000L;
        public const ulong WS_POPUP = 0x80000000L;
        public const ulong WS_CHILD = 0x40000000L;
        public const ulong WS_MINIMIZE = 0x20000000L;
        public const ulong WS_VISIBLE = 0x10000000L;
        public const ulong WS_DISABLED = 0x08000000L;
        public const ulong WS_CLIPSIBLINGS = 0x04000000L;
        public const ulong WS_CLIPCHILDREN = 0x02000000L;
        public const ulong WS_MAXIMIZE = 0x01000000L;
        public const ulong WS_CAPTION = 0x00C00000L;
        public const ulong WS_BORDER = 0x00800000L;
        public const ulong WS_DLGFRAME = 0x00400000L;
        public const ulong WS_VSCROLL = 0x00200000L;
        public const ulong WS_HSCROLL = 0x00100000L;
        public const ulong WS_SYSMENU = 0x00080000L;
        public const ulong WS_THICKFRAME = 0x00040000L;
        public const ulong WS_GROUP = 0x00020000L;
        public const ulong WS_TABSTOP = 0x00010000L;

        public const ulong WS_MINIMIZEBOX = 0x00020000L;
        public const ulong WS_MAXIMIZEBOX = 0x00010000L;

        public const uint LWA_COLORKEY = 0x00000001;
        public const uint LWA_ALPHA = 0x00000002;

        public const uint SWP_SHOWWINDOW = 0x0040;

        public const uint WM_NCLBUTTONUP = 0x00A2;
        public const uint WM_NCLBUTTONDOWN = 0x00A1;

        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_LBUTTONDBLCLK = 0x0203;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
        public const uint WM_RBUTTONDBLCLK = 0x0206;
        public const uint WM_MBUTTONDOWN = 0x0207;
        public const uint WM_MBUTTONUP = 0x0208;
        public const uint WM_MBUTTONDBLCLK = 0x0209;
        public const uint WM_MOUSEWHEEL = 0x020A;

        public const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
        public const int SW_SHOWMAXIMIZED = 3;//最大化
        public const int SW_SHOWRESTORE = 1;//还原

        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const uint WS_EX_TOPMOST = 0x00000008;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;
        const uint WS_EX_TOOLWINDOW = 0x00000080;//隐藏图标

        private static ulong _wndLng;
        private static IntPtr s_hwnd = IntPtr.Zero;

        public static IntPtr GetHWnd()
        {
            if (s_hwnd == IntPtr.Zero)
            {
#if !UNITY_EDITOR
                string UnityWindowClassName = "UnityWndClass";
                uint threadId = GetCurrentThreadId();
                EnumThreadWindows(threadId, (hWnd, lParam) =>
                {
                    var classText = new StringBuilder(UnityWindowClassName.Length + 1);
                    GetClassName(hWnd, classText, classText.Capacity);
                    Debug.Log(classText.ToString());
                    if (classText.ToString() == UnityWindowClassName)
                    {
                        s_hwnd = hWnd;
                        return false;
                    }
                    return true;
                }, IntPtr.Zero);
#endif
            }
            return s_hwnd;
        }

        static WinAPI()
        {
            s_hwnd = GetHWnd();
        }

        public static void SaveWindowLong()
        {
            _wndLng = GetWindowLong(s_hwnd, -16);
        }
        public static ulong LoadWindowLong()
        {
            return _wndLng;
        }

        public static void TransWindow()
        {
            s_hwnd = GetHWnd();
            Debug.LogError("fb:"+s_hwnd.ToString());
            Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
            var margins = new MARGINS() { cxLeftWidth = -1 };
            SetWindowLong(s_hwnd, GWL_STYLE, WS_EX_LAYERED | WS_POPUP | WS_VISIBLE);
            SetWindowLong(s_hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT); // 实现鼠标穿透
            DwmExtendFrameIntoClientArea(s_hwnd, ref margins);
        }

        public static void GameWindow()
        {
            s_hwnd = GetHWnd();
            var margins = new MARGINS() { };
            SetLayeredWindowAttributes(s_hwnd, 0, 255, LWA_ALPHA);
            SetWindowLong(s_hwnd, -16, WS_THICKFRAME | WS_VISIBLE);
            DwmExtendFrameIntoClientArea(s_hwnd, ref margins);
        }

        public static void NormalWindow()
        {
            s_hwnd = GetHWnd();
            var margins = new MARGINS() { };
            SetWindowLong(s_hwnd, -16,
                    WS_VISIBLE | WS_CAPTION | WS_THICKFRAME |
                    WS_MAXIMIZEBOX | WS_MINIMIZEBOX | WS_SYSMENU);
            DwmExtendFrameIntoClientArea(s_hwnd, ref margins);
        }
    }
}
