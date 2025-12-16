using FlagTip.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Utils
{
    internal static class NativeMethods
    {

        internal const int OBJID_CARET = -8;
		internal static RECT prevCaretLocation;


		[DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);


        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(
            IntPtr hwnd,
            int dwObjectID,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppvObject
        );


        internal static string GetProcessNameFromHwnd(IntPtr hwnd)
        {
            try
            {
                NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
                Process p = Process.GetProcessById((int)pid);
                return p.ProcessName.ToLower();
            }
            catch { return ""; }
        }


		internal static class ExplorerInfo
		{
			[DllImport("user32.dll")]
			static extern IntPtr GetForegroundWindow();

			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

			public static string GetForegroundWindowClassName()
			{
				IntPtr hwnd = GetForegroundWindow();
				StringBuilder className = new StringBuilder(256);
				GetClassName(hwnd, className, className.Capacity);
				return className.ToString();
			}
		}



		// GetClientRect -> 클라이언트 좌표계 반환 (0,0 ~ width,height)
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);








		// MOUSE HOOK 관련

		internal const int WH_MOUSE_LL = 14;

		internal enum MouseMessages
		{
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook,
			LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
			IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);


		internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }


        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);



    }
}
