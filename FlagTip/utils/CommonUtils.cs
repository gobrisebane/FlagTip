using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FlagTip.Utils.NativeMethods;
using static FlagTip.config.AppList;



namespace FlagTip.Utils
{
    internal static class CommonUtils
    {
        internal static bool IsRectValid(RECT r) => r.left != 0 || r.top != 0 || r.right != 0 || r.bottom != 0;



        public static bool IsProcessCursorApp()
        {
            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessName(hwnd);
            bool isCursorApp = CursorAppList.Contains(processName);
            return isCursorApp;
        }




        public static bool IsCaretInEditableArea(
    IntPtr hwnd,
    RECT caretRect)
        {


            if (hwnd == IntPtr.Zero)
                return false;

            // caret rect 유효성
            if (caretRect.right <= caretRect.left ||
                caretRect.bottom <= caretRect.top)
                return false;

            // client rect (0,0 ~ width,height)
            RECT clientRect;
            if (!GetClientRect(hwnd, out clientRect))
                return false;

            // client 좌표 → screen 좌표
            POINT clientTopLeft = new POINT { X = 0, Y = 0 };
            ClientToScreen(hwnd, ref clientTopLeft);

            RECT clientScreenRect = new RECT
            {
                left = clientTopLeft.X,
                top = clientTopLeft.Y,
                right = clientTopLeft.X + (clientRect.right - clientRect.left),
                bottom = clientTopLeft.Y + (clientRect.bottom - clientRect.top)
            };

            // 🔹 margin 적용
            const int TOP_MARGIN = 30; // 메뉴/툴바 제외
            const int BOTTOM_MARGIN = 20; // 상태표시줄 제외
            const int LEFT_MARGIN = 0;  // 필요시 라인번호 영역 제외
            const int RIGHT_MARGIN = 0;

            clientScreenRect.top += TOP_MARGIN;
            clientScreenRect.bottom -= BOTTOM_MARGIN;
            clientScreenRect.left += LEFT_MARGIN;
            clientScreenRect.right -= RIGHT_MARGIN;

            // caret 중심점 기준으로 판단 (살짝 겹쳐도 OK)
            int cx = (caretRect.left + caretRect.right) / 2;
            int cy = (caretRect.top + caretRect.bottom) / 2;

            return cx >= clientScreenRect.left &&
                   cx <= clientScreenRect.right &&
                   cy >= clientScreenRect.top &&
                   cy <= clientScreenRect.bottom;



        }


        public static bool IsKoreanIme()
        {
            IntPtr hwnd = GetForegroundWindow();
            IntPtr hIMC = ImmGetContext(hwnd);

            if (hIMC == IntPtr.Zero)
                return false;

            ImmGetConversionStatus(hIMC, out int conversion, out int sentence);
            ImmReleaseContext(hwnd, hIMC);

            Console.WriteLine("IME_CMODE_NATIVE : " + IME_CMODE_NATIVE);

            // IME_CMODE_NATIVE = 한글
            return (conversion & IME_CMODE_NATIVE) != 0;
        }


        public static bool IsKorean()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return false;

            IntPtr imeWnd = ImmGetDefaultIMEWnd(hwnd);
            if (imeWnd == IntPtr.Zero)
                return false;

            IntPtr mode = SendMessage(
                imeWnd,
                WM_IME_CONTROL,
                (IntPtr)IMC_GETCONVERSIONMODE,
                IntPtr.Zero);

            int conversion = mode.ToInt32();

            // IME_CMODE_NATIVE = 0x1
            return (conversion & 0x1) != 0;
        }




        public static ImeState GetImeState()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return ImeState.ENG;

            IntPtr imeWnd = ImmGetDefaultIMEWnd(hwnd);
            if (imeWnd == IntPtr.Zero)
                return ImeState.ENG;

            IntPtr mode = SendMessage(
                imeWnd,
                WM_IME_CONTROL,
                (IntPtr)IMC_GETCONVERSIONMODE,
                IntPtr.Zero);

            int conversion = mode.ToInt32();

            // IME_CMODE_NATIVE = 0x1
            return (conversion & 0x1) != 0
                ? ImeState.KOR
                : ImeState.ENG;
        }




    }
}
