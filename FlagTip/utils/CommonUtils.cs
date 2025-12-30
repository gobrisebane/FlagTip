using FlagTip.Input.Native;
using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.config.AppList;
using static FlagTip.Utils.NativeMethods;
//using static FlagTip.Input.Native.Imm32;
using static FlagTip.Input.Native.User32;



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
    RECT caretRect,
    CaretMethod caretMethod)
        {

            if (caretMethod == CaretMethod.UIA || caretMethod == CaretMethod.UIAExplorer)
                return true;


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
            const int TOP_MARGIN = 0; 
            const int BOTTOM_MARGIN = 0; 
            const int LEFT_MARGIN = 0; 
            const int RIGHT_MARGIN = 0;

            clientScreenRect.top += TOP_MARGIN;
            clientScreenRect.bottom -= BOTTOM_MARGIN;
            clientScreenRect.left += LEFT_MARGIN;
            clientScreenRect.right -= RIGHT_MARGIN;

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


      


        internal static ImeState GetImeState()
        {

            IntPtr fg = GetForegroundWindow();
            if (fg == IntPtr.Zero)
                return ImeState.ENG;

            uint targetTid = GetWindowThreadProcessId(fg, IntPtr.Zero);

            GUITHREADINFO info = new GUITHREADINFO
            {
                cbSize = Marshal.SizeOf<GUITHREADINFO>()
            };

            if (!User32.GetGUIThreadInfo((int)targetTid, ref info))
                return ImeState.ENG;

            IntPtr hwnd = info.hwndFocus != IntPtr.Zero
                ? info.hwndFocus
                : fg;

            //if (info.hwndFocus != IntPtr.Zero)
            //{
            //    hwnd = info.hwndFocus;
            //}

            int r = IsKoreanIMEUsingIMM32(hwnd);

            if (r == 1)
                return ImeState.KOR;

            if (r == 0)
                return ImeState.ENG;

            return ImeState.ENG;
        }



        internal static int IsKoreanIMEUsingIMM32(IntPtr hWnd)
        {
            const int IMC_GETCONVERSIONMODE = 0x1;
            const int IME_CMODE_NATIVE = 0x1;

            IntPtr hIMC = IntPtr.Zero;
            int result = -1;

            try
            {
                if (hWnd == IntPtr.Zero)
                    return -1;

                // 1 / ImmGetContext
                hIMC = Imm32.ImmGetContext(hWnd);
                if (hIMC != IntPtr.Zero)
                {
                    if (!Imm32.ImmGetConversionStatus(hIMC, out int conversion, out _))
                        return -1;

                    result = (conversion & IME_CMODE_NATIVE) != 0 ? 1 : 0;
                    return result;
                }

                //  2 / Default IME Window + WM_IME_CONTROL
                IntPtr hImeWnd = Imm32.ImmGetDefaultIMEWnd(hWnd);
                if (hImeWnd != IntPtr.Zero)
                {
                    IntPtr msgResult;

                    IntPtr sendResult = User32.SendMessageTimeout(
                        hImeWnd,
                        User32.WM_IME_CONTROL,
                        (IntPtr)IMC_GETCONVERSIONMODE,
                        IntPtr.Zero,
                        User32.SMTO_NORMAL |
                        User32.SMTO_ABORTIFHUNG |
                        User32.SMTO_ERRORONEXIT,
                        1000,
                        out msgResult);

                    if (sendResult == IntPtr.Zero)
                        return -1;

                    result = ((int)msgResult & IME_CMODE_NATIVE) != 0 ? 1 : 0;
                    return result;
                }

                return -1;
            }
            finally
            {
                if (hIMC != IntPtr.Zero)
                    Imm32.ImmReleaseContext(hWnd, hIMC);
            }
        }





        internal static ImeState GetImeState5()
        {
            IntPtr fg = GetForegroundWindow();
            if (fg == IntPtr.Zero)
                return ImeState.ENG;

            uint targetTid = GetWindowThreadProcessId(fg, IntPtr.Zero);

            GUITHREADINFO info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(typeof(GUITHREADINFO));

            if (!User32.GetGUIThreadInfo((int)targetTid, ref info))
                return ImeState.ENG;

            IntPtr hwnd = info.hwndFocus;
            if (hwnd == IntPtr.Zero)
                return ImeState.ENG;

            IntPtr hIMC = Imm32.ImmGetContext(hwnd);
            if (hIMC == IntPtr.Zero)
                return ImeState.ENG;

            try
            {
                bool imeOpen = Imm32.ImmGetOpenStatus(hIMC);
                Imm32.ImmGetConversionStatus(hIMC, out int conversion, out _);

                bool isKorean =
                    imeOpen &&
                    (conversion & IME_CMODE_NATIVE) != 0;

                return isKorean ? ImeState.KOR : ImeState.ENG;
            }
            finally
            {
                Imm32.ImmReleaseContext(hwnd, hIMC);
            }
        }



        internal static ImeState GetImeState4()
        {
            // 1️⃣ 현재 포커스된 실제 입력 HWND 얻기
            GUITHREADINFO info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(typeof(GUITHREADINFO));

            if (!User32.GetGUIThreadInfo(0, ref info))
                return ImeState.ENG;

            IntPtr hwnd = info.hwndFocus;
            if (hwnd == IntPtr.Zero)
                return ImeState.ENG;

            // 2️⃣ IME Context 획득
            IntPtr hIMC = Imm32.ImmGetContext(hwnd);
            if (hIMC == IntPtr.Zero)
                return ImeState.ENG;

            try
            {
                // 3️⃣ IME 열림 여부
                bool imeOpen = Imm32.ImmGetOpenStatus(hIMC);

                // 4️⃣ 변환 모드
                Imm32.ImmGetConversionStatus(hIMC, out int conversion, out _);

                bool isKorean =
                    imeOpen &&
                    (conversion & IME_CMODE_NATIVE) != 0;

                return isKorean
                    ? ImeState.KOR
                    : ImeState.ENG;
            }
            finally
            {
                Imm32.ImmReleaseContext(hwnd, hIMC);
            }
        }

        public static ImeState GetImeState3()
        {
            GUITHREADINFO info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);

            if (!GetGUIThreadInfo(0, ref info))
                return ImeState.ENG;

            IntPtr hwnd = info.hwndFocus;
            if (hwnd == IntPtr.Zero)
                return ImeState.ENG;

            IntPtr hIMC = ImmGetContext(hwnd);
            if (hIMC == IntPtr.Zero)
                return ImeState.ENG;

            try
            {
                // 1️⃣ IME 열림 여부
                bool imeOpen = ImmGetOpenStatus(hIMC);

                // 2️⃣ 변환 모드
                ImmGetConversionStatus(hIMC, out int conversion, out _);

                bool isKorean =
                    imeOpen &&
                    (conversion & IME_CMODE_NATIVE) != 0;

                return isKorean ? ImeState.KOR : ImeState.ENG;
            }
            finally
            {
                ImmReleaseContext(hwnd, hIMC);
            }
        }


       /* public static ImeState GetImeState_2()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return ImeState.ENG;

            IntPtr hIMC = ImmGetContext(hwnd);
            if (hIMC == IntPtr.Zero)
                return ImeState.ENG;

            ImmGetConversionStatus(hIMC, out int conversion, out _);
            ImmReleaseContext(hwnd, hIMC);

            return (conversion & IME_CMODE_NATIVE) != 0
                ? ImeState.KOR
                : ImeState.ENG;
        }*/

       /* public static ImeState GetImeState()
        {
            IntPtr fg = GetForegroundWindow();
            if (fg == IntPtr.Zero)
                return ImeState.ENG;

            //IntPtr hwnd = GetForegroundWindow();
            GUITHREADINFO info = new GUITHREADINFO
            {
                cbSize = Marshal.SizeOf<GUITHREADINFO>()
            };

            IntPtr hwnd = info.hwndFocus != IntPtr.Zero
                 ? info.hwndFocus
                 : fg;
            //if (hwnd == IntPtr.Zero)
            //    return ImeState.ENG;

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
*/

        public static ImeState GetImeState1_bk()
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


