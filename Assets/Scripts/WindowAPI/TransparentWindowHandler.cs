using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace WINDOWAPI
{
    public class TransparentWindowHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_forceRuninBackground = false;

        //===============================================================

        private void Start()
        {
            //MessageBox(new IntPtr(0), "Hello World!", "Hello Dialog", 0);

#if !UNITY_EDITOR
            hWnd = GetActiveWindow();
            MARGINS margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(hWnd, ref margins);

            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

            if (!this.m_forceRuninBackground) return;
            Application.runInBackground = true; //Force run in background
        }

        private void Update()
        {
            SetClickthrough(Physics2D.OverlapPoint(KC_Custom.KCUtil.GetMouseWorldPosition()) == null);
        }

        //===============================================================

        //This is to display the message window box only
        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("Dwmapi.dll")]
        private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        const uint LWA_COLORKEY = 0x00000001;

        IntPtr hWnd;

        private void SetClickthrough(bool clickThrough)
        {
            if (clickThrough)
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            }
        }

        //===============================================================
    }
}