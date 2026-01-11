using FlagTip.Input.Native;
using FlagTip.Models;
using FlagTip.Utils;
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

            ImeState imeResult = ImeState.UNKNOWN;


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
            {
                imeResult = ImeState.KOR;
            } else if (r == 0)
            {
                if (CommonUtils.IsCapsLockOn())
                    imeResult = ImeState.ENG_UP;
                else
                    imeResult = ImeState.ENG_LO;
            }


            return imeResult;


        }

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("imm32.dll")]
        static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint lpdwConversion, out uint lpdwSentence);

        [DllImport("kernel32.dll")]
static extern uint GetCurrentThreadId(); // 최신 방식

public static ImeState GetImeMode()
{
    IntPtr hwnd = GetForegroundWindow();
    if (hwnd == IntPtr.Zero) return ImeState.UNKNOWN;

    // 1. 네이티브 스레드 ID 가져오기
    uint targetThread = GetWindowThreadProcessId(hwnd, out _);
    uint currentThread = GetCurrentThreadId(); // 수정된 부분

    // 2. 입력 스레드 연결 (포커스된 컨트롤의 정보를 얻기 위함)
    bool attached = AttachThreadInput(currentThread, targetThread, true);
    
    try
    {
        // 실제로 포커스가 있는 자식 윈도우(입력창)를 찾음
        IntPtr focusedHandle = GetFocus();
        IntPtr targetHandle = (focusedHandle == IntPtr.Zero) ? hwnd : focusedHandle;
        
        IntPtr hIMC = ImmGetContext(targetHandle);
        if (hIMC != IntPtr.Zero)
        {
            uint conversion, sentence;
            bool success = ImmGetConversionStatus(hIMC, out conversion, out sentence);
            ImmReleaseContext(targetHandle, hIMC);

            if (success)
            {
                return (conversion & 0x0001) != 0 ? ImeState.KOR : ImeState.ENG_LO;
            }
        }
    }
    finally
    {
        // 연결 해제는 반드시 수행되어야 함
        if (attached) AttachThreadInput(currentThread, targetThread, false);
    }

    return ImeState.UNKNOWN;
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
