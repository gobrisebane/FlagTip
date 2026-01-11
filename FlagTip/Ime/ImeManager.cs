using FlagTip.Models;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using System.Windows.Automation;
using System.Windows.Forms;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Ime
{

    public class ImeManager
    {
        // --- Win32 API 선언 ---
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("imm32.dll")]
        static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        // IME 메시지 상수
        const uint WM_IME_CONTROL = 0x0283;
        const uint IMC_GETCONVERSIONMODE = 0x0001;





        // 필요한 추가 Win32 선언
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam,
            SendMessageTimeoutFlags flags, uint timeout, out UIntPtr lpdwResult);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint lpdwConversion, out uint lpdwSentence);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);





        // KEEP 버전
        private const uint IMC_GETCONVERSIONSTATUS = 0x0001;









       

        //안됨

        // IME 변환 모드 상수
        private const uint IME_CMODE_HANGEUL = 0x0001;
        private const uint IME_CMODE_NATIVE = 0x0001; // HANGEUL과 동일

        public static ImeState GetChromeImeMode()
        {
            try
            {
                IntPtr foregroundWnd = GetForegroundWindow();
                if (foregroundWnd == IntPtr.Zero) return ImeState.UNKNOWN;

                uint threadId = GetWindowThreadProcessId(foregroundWnd, out _);
                GUITHREADINFO guiInfo = new GUITHREADINFO();
                guiInfo.cbSize = Marshal.SizeOf(guiInfo);

                if (GetGUIThreadInfo(threadId, ref guiInfo))
                {
                    // hwndFocus가 없으면 hwndActive를 사용
                    IntPtr targetHandle = (guiInfo.hwndFocus != IntPtr.Zero) ? guiInfo.hwndFocus : foregroundWnd;

                    IntPtr hIMC = ImmGetContext(targetHandle);
                    if (hIMC != IntPtr.Zero)
                    {
                        uint conversion, sentence;
                        // 상태를 가져와서 비트 연산으로 한글 모드인지 확인
                        if (ImmGetConversionStatus(hIMC, out conversion, out sentence))
                        {
                            ImmReleaseContext(targetHandle, hIMC);

                            // conversion 비트에 IME_CMODE_HANGEUL(1)이 포함되어 있으면 한글
                            return (conversion & IME_CMODE_HANGEUL) != 0 ? ImeState.KOR : ImeState.ENG_LO;
                        }
                        ImmReleaseContext(targetHandle, hIMC);
                    }
                }
            }
            catch { }
            return ImeState.UNKNOWN;
        }













        public static ImeState GetChromeImeMode8()
        {
            try
            {
                Console.WriteLine("hello");
                // 1. 현재 UI Automation이 포커스하고 있는 요소 가져오기
                AutomationElement focusedElement = AutomationElement.FocusedElement;

                if (focusedElement == null) return ImeState.UNKNOWN;

                // 2. 해당 요소의 Native 윈도우 핸들(HWND) 가져오기
                IntPtr handle = new IntPtr(focusedElement.Current.NativeWindowHandle);

                if (handle != IntPtr.Zero)
                {
                    // 3. IME 컨텍스트를 얻어와서 상태 확인
                    IntPtr hIMC = ImmGetContext(handle);
                    if (hIMC != IntPtr.Zero)
                    {
                        bool isHangul = ImmGetOpenStatus(hIMC);
                        ImmReleaseContext(handle, hIMC);

                        return (isHangul) ? ImeState.KOR : ImeState.ENG_LO;
                    }
                }
            }
            catch (Exception ex)
            {
                // UI Automation 접근 중 요소가 사라지거나 에러가 날 경우 처리
                Console.WriteLine($"Error checking IME: {ex.Message}");
            }

            return ImeState.UNKNOWN;
        }



        // 작동안됨
        public static ImeState GetChromeImeMode7()
        {
            IntPtr foregroundWnd = GetForegroundWindow();

            // 1. IME 컨텍스트 가져오기
            // 일반적인 경우 GetForegroundWindow를 쓰지만, 
            // 크롬 내부 입력창의 정확한 상태를 위해 ImmGetContext를 호출합니다.
            IntPtr hIMC = ImmGetContext(foregroundWnd);

            try
            {
                // 2. 상태 확인
                // true 반환 시 한글(조합) 모드, false 반환 시 영문 모드
                return (ImmGetOpenStatus(hIMC)) ? ImeState.KOR : ImeState.ENG_LO;
            }
            finally
            {
                // 3. 반드시 컨텍스트 해제
                ImmReleaseContext(foregroundWnd, hIMC);
            }
        }





        public static ImeState GetChromeImeMode6()
        {
            IntPtr hwnd = GetForegroundWindow(); // 현재 활성화된 창
            IntPtr hImeWnd = ImmGetDefaultIMEWnd(hwnd);

            // 메인 스레드에서 메시지를 보내 상태 확인
            uint status = (uint)SendMessage(hImeWnd, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONSTATUS, IntPtr.Zero);

            // status & 1 이 1이면 한글, 0이면 영어
            return (status & 1) != 0 ? ImeState.KOR : ImeState.ENG_LO;
        }

        


        public static ImeState GetChromeImeMode5()
        {


            string keyPath = @"Software\Microsoft\IME\15.0\IMEKR";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    if (key != null)
                    {
                        // 1이면 구버전(호환성 모드) 사용, 0이면 최신 버전 사용
                        key.SetValue("TSFCompatibilityMode", true ? 1 : 0, RegistryValueKind.DWord);
                    }
                }

                // 주의: 레지스트리 변경 후 즉시 적용되지 않을 수 있습니다.
                // 해당 앱(크롬 등)을 재시작하거나 윈도우 로그아웃이 필요할 수 있습니다.
            }
            catch (Exception ex)
            {
                Console.WriteLine("레지스트리 수정 권한이 없습니다: " + ex.Message);
            }



            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero) return ImeState.UNKNOWN;

            uint threadId = GetWindowThreadProcessId(foregroundHwnd, out _);
            GUITHREADINFO guiInfo = new GUITHREADINFO();
            guiInfo.cbSize = Marshal.SizeOf(guiInfo);

            IntPtr targetHwnd = foregroundHwnd;

            // GUI 정보를 통해 실제 포커스된 하위 윈도우(RenderWidgetHost)를 찾음
            if (GetGUIThreadInfo(threadId, ref guiInfo))
            {
                if (guiInfo.hwndFocus != IntPtr.Zero) targetHwnd = guiInfo.hwndFocus;
            }

            // Default IME Window 찾기
            IntPtr imeWnd = ImmGetDefaultIMEWnd(targetHwnd);
            if (imeWnd == IntPtr.Zero) return ImeState.UNKNOWN;

            // SendMessage 대신 SendMessageTimeout 사용 (응답 없으면 20ms 내에 포기)
            // 먹통 현상을 방지하는 핵심 장치입니다.
            UIntPtr dwResult;
            IntPtr ret = SendMessageTimeout(imeWnd, 0x0283, (IntPtr)0x0001, IntPtr.Zero,
                                            SendMessageTimeoutFlags.SMTO_ABORTIFHUNG | SendMessageTimeoutFlags.SMTO_NOTIMEOUTIFNOTHUNG,
                                            20, out dwResult);

            if (ret != IntPtr.Zero)
            {
                return (dwResult.ToUInt64() & 0x0001) != 0 ? ImeState.KOR : ImeState.ENG_LO;
            }

            return ImeState.UNKNOWN;
        }


       /* public static ImeState GetChromeImeMode4()
        {
            // 1. 현재 최상위 창과 포커스된 스레드 정보 가져오기
            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero) return ImeState.UNKNOWN;

            uint threadId = GetWindowThreadProcessId(foregroundHwnd, out _);
            GUITHREADINFO guiInfo = new GUITHREADINFO();
            guiInfo.cbSize = Marshal.SizeOf(guiInfo);

            IntPtr targetHwnd = foregroundHwnd;
            if (GetGUIThreadInfo(threadId, ref guiInfo))
            {
                if (guiInfo.hwndCaret != IntPtr.Zero) targetHwnd = guiInfo.hwndCaret;
                else if (guiInfo.hwndFocus != IntPtr.Zero) targetHwnd = guiInfo.hwndFocus;
            }

            // --- 방법 A: ImmGetContext를 이용한 직접 접근 (가장 정확함) ---
            // 특정 앱(Chrome 등)은 이 방식이 SendMessage보다 더 즉각적입니다.
            IntPtr hIMC = ImmGetContext(targetHwnd);
            if (hIMC != IntPtr.Zero)
            {
                int conversion, sentence; // uint에서 int로 변경
                if (ImmGetConversionStatus(hIMC, out conversion, out sentence))
                {
                    ImmReleaseContext(targetHwnd, hIMC);
                    // 0x0001 (IME_CMODE_NATIVE) 검사
                    return (conversion & 0x0001) != 0 ? ImeState.KOR : ImeState.ENG_LO;
                }
                ImmReleaseContext(targetHwnd, hIMC);
            }

            // --- 방법 B: ImmGetDefaultIMEWnd + SendMessage (Fallback) ---
            // 방법 A가 실패할 경우 기존에 작성하신 메시지 방식으로 보완합니다.
            IntPtr imeWnd = ImmGetDefaultIMEWnd(targetHwnd);
            if (imeWnd == IntPtr.Zero && targetHwnd != foregroundHwnd)
            {
                imeWnd = ImmGetDefaultIMEWnd(foregroundHwnd);
            }

            if (imeWnd != IntPtr.Zero)
            {
                UIntPtr dwResult;
                // 응답 없음 방지를 위해 SendMessageTimeout 권장
                IntPtr ret = SendMessageTimeout(imeWnd, 0x0283, (IntPtr)0x0001, IntPtr.Zero,
                                                SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 50, out dwResult);

                if (ret != IntPtr.Zero)
                {
                    return (dwResult.ToUInt64() & 0x0001) != 0 ? ImeState.KOR : ImeState.ENG_LO;
                }
            }

            return ImeState.UNKNOWN;
        }*/


        public static ImeState GetChromeImeMode2()
        {
            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero) return ImeState.UNKNOWN;

            // 1. GUI 정보를 가져와서 실제 "커서"가 있는 핸들을 찾습니다.
            uint threadId = GetWindowThreadProcessId(foregroundHwnd, out _);
            GUITHREADINFO guiInfo = new GUITHREADINFO();
            guiInfo.cbSize = Marshal.SizeOf(guiInfo);

            IntPtr realInputHwnd = foregroundHwnd;
            if (GetGUIThreadInfo(threadId, ref guiInfo))
            {
                // 커서(Caret) -> 포커스(Focus) 순으로 핸들 우선순위 부여
                if (guiInfo.hwndCaret != IntPtr.Zero) realInputHwnd = guiInfo.hwndCaret;
                else if (guiInfo.hwndFocus != IntPtr.Zero) realInputHwnd = guiInfo.hwndFocus;
            }

            // 2. 찾아낸 실제 입력 핸들의 Default IME 창을 찾습니다.
            IntPtr imeWnd = ImmGetDefaultIMEWnd(realInputHwnd);

            // 3. 만약 실패하면 메인 창의 Default IME 창이라도 시도합니다.
            if (imeWnd == IntPtr.Zero)
                imeWnd = ImmGetDefaultIMEWnd(foregroundHwnd);

            if (imeWnd != IntPtr.Zero)
            {
                // 메시지를 보내 현재 모드를 가져옵니다.
                IntPtr conversionMode = SendMessage(imeWnd, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONMODE, IntPtr.Zero);
                long mode = conversionMode.ToInt64();

                // 윈도우 11의 '한글' 상태 비트 검사
                // 0x01: Native 모드(한글), 0x00: Alpha 모드(영어)
                return (mode & 0x0001) != 0 ? ImeState.KOR : ImeState.ENG_LO;
            }

            return ImeState.UNKNOWN;
        }


        public static ImeState GetChromeImeMode1()
        {
            // 1. 현재 활성화된 창(크롬) 핸들을 가져옵니다.
            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero) return ImeState.UNKNOWN;

            // 2. 해당 창의 '기본 IME 관리 창' 핸들을 가져옵니다.
            // 이 창은 실제 입력창 뒤에서 IME 상태를 관리하는 숨겨진 창입니다.
            IntPtr imeWnd = ImmGetDefaultIMEWnd(foregroundHwnd);

            if (imeWnd == IntPtr.Zero)
            {
                // 때로는 자식 윈도우 핸들이 필요할 수 있으므로 
                // GetGUIThreadInfo에서 얻은 hwndFocus를 여기에 대입할 수도 있습니다.
                return ImeState.UNKNOWN;
            }

            // 3. IME 창에 직접 메시지를 보내 현재 변환 모드(한/영)를 가져옵니다.
            // 이 방식은 Context를 직접 열지 않아도 되므로 ReleaseContext가 필요 없습니다.
            IntPtr conversionMode = SendMessage(imeWnd, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONMODE, IntPtr.Zero);

            // 4. 결과 해석
            // conversionMode가 0이면 영어, 1(또는 1을 포함한 비트)이면 한글입니다.
            long mode = conversionMode.ToInt64();

            // 윈도우 11/크롬 대응 비트 연산
            if ((mode & 0x0001) != 0)
            {
                return ImeState.KOR;
            }
            else
            {
                return ImeState.ENG_LO;
            }
        }

       
    }

}