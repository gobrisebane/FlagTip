using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UIA;

namespace FlagTip.Utils
{
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int width => right - left;
            public int height => bottom - top;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }



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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        internal static string GetProcessName(IntPtr hwnd)
        {
            try
            {
                NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
                Process p = Process.GetProcessById((int)pid);
                return p.ProcessName.ToLower();
            }
            catch { return ""; }
        }

        internal static uint GetForegroundProcessId()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return 0;

            GetWindowThreadProcessId(hwnd, out uint pid);
            return pid;
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
            WM_MOUSEHWHEEL = 0x020E,
            WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208, 
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C
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



        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            IntPtr lParam
        );

        const int WM_NCHITTEST = 0x0084;
        const int HTCAPTION = 2;


        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(
            IntPtr hWnd,
            out RECT lpRect
        );







        // IME V1

        internal const int IME_CMODE_NATIVE = 0x0001;

        [DllImport("imm32.dll")]
        internal static extern bool ImmGetOpenStatus(IntPtr hIMC);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll")]
        internal static extern bool ImmGetConversionStatus(
            IntPtr hIMC,
            out int lpfdwConversion,
            out int lpfdwSentence
        );

        //internal static class User32
        //{
        //    [DllImport("user32.dll")]
        //    internal static extern bool GetGUIThreadInfo(
        //        int idThread,
        //        ref GUITHREADINFO lpgui);
        //}




        // IME V2

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(
            IntPtr hWnd,
            IntPtr lpdwProcessId);


        internal static bool IsKoreanLayout()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint tid = GetWindowThreadProcessId(hwnd, IntPtr.Zero);

            IntPtr hkl = GetKeyboardLayout(tid);

            int langId = hkl.ToInt32() & 0xFFFF;
            return langId == 0x0412;
        }



        // IME V3
        //internal const int WM_IME_CONTROL = 0x283;
        internal const int IMC_GETCONVERSIONMODE = 0x001;


        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);









        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(
            string lpClassName,
            string lpWindowName);

   




        // WinEventHook

        internal const uint EVENT_OBJECT_FOCUS = 0x8005;
        internal const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        internal const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        internal delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime
        );

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(
        uint eventMin,
        uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags
    );

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);



        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromEvent(
            IntPtr hwnd,
            int idObject,
            int idChild,
            [Out, MarshalAs(UnmanagedType.Interface)] out IAccessible acc,
            out object varChild
        );

    }
}
