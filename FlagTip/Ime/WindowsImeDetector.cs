using FlagTip.Input.Native;
using FlagTip.Input.Tsf;
using FlagTip.Models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                return ImeState.ENG_LO;

            uint targetTid = GetWindowThreadProcessId(fg, IntPtr.Zero);

            GUITHREADINFO info = new GUITHREADINFO
            {
                cbSize = Marshal.SizeOf<GUITHREADINFO>()
            };

            if (!User32.GetGUIThreadInfo((int)targetTid, ref info))
                return ImeState.ENG_LO;

            IntPtr hwnd = info.hwndFocus != IntPtr.Zero
                ? info.hwndFocus
                : fg;

            // 🔑 1. 언어 판별 (HKL)
            IntPtr hkl = GetKeyboardLayout(targetTid);
            ushort langId = LOWORD(hkl);

            switch (langId)
            {
                case 0x0412: // Korean
                    {
                        
                        int r = IsKoreanIMEUsingIMM32(hwnd);
                        if (r == 1)
                        {
                            imeResult = ImeState.KOR;
                        }
                        else if (r == 0)
                        {
                            if (CommonUtils.IsCapsLockOn())
                                imeResult = ImeState.ENG_UP;
                            else
                                imeResult = ImeState.ENG_LO;
                        }

                        break;
                    }



                //case 0x0411: // Japanese
                //    imeResult = ImeState.JPN;
                //    break;




                case 0x0411: // Japanese
                    {

                        //bool isNative = IsJapaneseNativeMode(hwnd);

                        //Console.WriteLine("isNative : " + isNative);

                        //imeResult = isNative
                        //    ? ImeState.JPN        // 히라가나/가타카나 모드
                        //    : ImeState.ENG_LO;    // 반각 영수(Direct Input) 모드


                        /*
                        bool isJapanese = JapaneseImeTsfHelper.IsJapaneseNative();

                        imeResult = isJapanese
                            ? ImeState.JPN        // 히라가나/가타카나 모드
                            : ImeState.ENG_LO;    // 반각 영수(Direct Input) 모드

                        Console.WriteLine(isJapanese ? "あ (Japanese)" : "A (English)");

                        //String test = GetComposition.CurrentCompStr(hwnd);

                        var comp = new GetComposition();
                        string str = comp.CurrentCompStr(hwnd);
                        Console.WriteLine(">>> str : " + str);
                        */


                        int r = IsKoreanIMEUsingIMM32(hwnd);
                        imeResult = (r == 1) ? ImeState.JPN : ImeState.ENG_LO;

                        break;
                    }



                case 0x0409: // English
                default:
                    imeResult = CommonUtils.IsCapsLockOn()
                        ? ImeState.ENG_UP
                        : ImeState.ENG_LO;
                    break;
            }

            return imeResult;
        }


        private static bool IsJapaneseNativeMode(IntPtr hwnd)
        {
            IntPtr hIMC = ImmGetContext(hwnd);
            if (hIMC == IntPtr.Zero) return false;

            try
            {
                if (ImmGetConversionStatus(hIMC, out uint conversion, out _))
                {
                    // IME가 켜져 있는지 확인 (0이면 Direct Input/영어 모드)
                    // 일본어의 경우 Native 모드(0x0001)가 켜져 있어야 히라가나 입력 상태입니다.
                    const uint IME_CMODE_NATIVE = 0x0001;

                    // conversion 값이 0보다 크고 Native 비트가 서 있다면 일본어 입력 상태로 간주
                    return (conversion & IME_CMODE_NATIVE) != 0;
                }
            }
            finally
            {
                ImmReleaseContext(hwnd, hIMC);
            }

            return false;
        }

        






        internal static ImeState GetWindowsImeState2()
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
            }
            else if (r == 0)
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


        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        static ushort LOWORD(IntPtr value)
        {
            return (ushort)((ulong)value & 0xFFFF);
        }

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
