using FlagTip.Input.Native;
using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Ime
{
    internal class WindowsImeDetector
    {

        internal static ImeState GetWindowsImeState()
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



    }
}
