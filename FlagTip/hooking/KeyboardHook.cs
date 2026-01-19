using FlagTip.Caret;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.Hooking
{
    internal static class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private const uint LLKHF_REPEAT = 0x40000000;

        private static bool _ctrlUp;
        private static bool _ctrlDown;
        private static bool _shiftDown;
        private static bool _altDown;
        private static bool _winDown;
        private static bool _spaceDown;


        private static long _lastTriggerTick;
        private const int TRIGGER_INTERVAL_MS = 40;
        private static bool _hangulKeyPressed = false;
        private static bool _capsLockKeyPressed = false;


        private static bool _jpImeKeyPressed = false;
        private const int VK_KANA = 0x15; // かな
        private const int VK_CONVERT = 0x1C; // 変換
        private const int VK_NONCONVERT = 0x1D; // 無変換
        private const int VK_KANJI = 0xF3; // 半角/全角
        private const int VK_LMENU = 0xA4;
        private const int VK_RMENU = 0xA5;
        private const int VK_CAPITAL = 0x14;


        private static readonly HashSet<Keys> CaretKeys = new HashSet<Keys>
        {
            Keys.Enter,
            Keys.Back,
            Keys.Delete,
            Keys.Home,
            Keys.End,
            Keys.F2,
            Keys.PageUp,
            Keys.PageDown,
            Keys.Tab,
            Keys.Insert,
            Keys.Escape,
            Keys.Left,
            Keys.Right,
            Keys.Up,
            Keys.Down
        };







        internal static IntPtr KeyboardHookCallback(
            int nCode,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr hookID,
            CaretController caretController)
        {
            if (nCode < 0)
                return CallNextHookEx(hookID, nCode, wParam, lParam);

            var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var key = (Keys)info.vkCode;

            bool isKeyDown =
                wParam == (IntPtr)WM_KEYDOWN ||
                wParam == (IntPtr)WM_SYSKEYDOWN;

            bool isKeyUp =
                wParam == (IntPtr)WM_KEYUP ||
                wParam == (IntPtr)WM_SYSKEYUP;

            // ---- modifier 상태 추적 (L/R 포함) ----
            if (isKeyDown || isKeyUp)
            {
                bool up = isKeyUp;
                bool down = isKeyDown;

                switch (key)
                {
                    case Keys.ControlKey:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        _ctrlUp = up;
                        _ctrlDown = down;
                        break;

                    case Keys.ShiftKey:
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                        _shiftDown = down;
                        break;

                    case Keys.Menu:
                    case Keys.LMenu:
                    case Keys.RMenu:
                        _altDown = down;
                        break;

                    case Keys.Space:
                        _spaceDown = down;
                        break;

                    case Keys.LWin:
                    case Keys.RWin:
                        _winDown = down;
                        break;
                }
            }







            if (key == Keys.HangulMode)
            {
                if (isKeyDown)
                {
                    if (_hangulKeyPressed)
                    {
                        return CallNextHookEx(hookID, nCode, wParam, lParam);
                    }

                    _hangulKeyPressed = true;
                    _ = caretController.NotifyImeToggle();
                }
                else
                {
                    _hangulKeyPressed = false;
                    _ = caretController.NotifyImeToggle();
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }



            if (key == Keys.CapsLock)
            {
                if (isKeyDown)
                {
                    // --- dual ---
                    if (_ctrlDown || _altDown || _winDown)
                    {
                        if (_capsLockKeyPressed)
                            return CallNextHookEx(hookID, nCode, wParam, lParam);

                        _capsLockKeyPressed = true;

                        _ = caretController.NotifyCapsLockToggle();
                        return CallNextHookEx(hookID, nCode, wParam, lParam);
                    }

                    // --- single ---
                    if (_capsLockKeyPressed)
                        return CallNextHookEx(hookID, nCode, wParam, lParam);

                    _capsLockKeyPressed = true;

                    _ = caretController.NotifyCapsLockToggle();
                }
                else
                {
                    // KeyUp 에서 플래그만 해제
                    _capsLockKeyPressed = false;
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }








            if (isKeyUp && IsTypingKey(key))
            {
                // return
                caretController.NotifyTyping();
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }

            // KeyUp 은 여기서 종료
            if (!isKeyDown)
                return CallNextHookEx(hookID, nCode, wParam, lParam);

            //---- auto-repeat 차단 (꾹 누르기 방지)----
            if ((info.flags & LLKHF_REPEAT) != 0)
                return CallNextHookEx(hookID, nCode, wParam, lParam);

            // ---- debounce ----
            long now = Environment.TickCount;
            if (now - _lastTriggerTick < TRIGGER_INTERVAL_MS)
                return CallNextHookEx(hookID, nCode, wParam, lParam);

            _lastTriggerTick = now;




            


            if (CaretKeys.Contains(key))
            {
                caretController.NotifyCaretMove();
                TriggerSafe(() => caretController.OnKeyChangedAsync());

                if (key == Keys.Enter)
                {
                    TriggerSafe(() => caretController.MultiSelectModeBrowser(8));
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }





            if (_ctrlDown || _altDown || _shiftDown || _winDown)
            {

                // ---- 단축키 ----
                if (_ctrlDown && key == Keys.Y)
                {
                    //TEST
                    TriggerSafe(caretController.OnKeyTest);
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                


                    if (_ctrlDown && key == Keys.A)
                {
                    caretController.NotifySelectAll();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }




                if ((_altDown && _shiftDown) || (_winDown && _spaceDown))
                {
                    _ = caretController.NotifyImeToggleKorJpn();
                }

                if (_ctrlDown || _altDown)
                {
                    TriggerSafe(() => caretController.OnKeyChangedAsync());
                }


                if (_winDown)
                {

                    //TriggerSafe(() => caretController.MultiSelectMode());
                    TriggerSafe(() => caretController.MultiSelectMode(5));

                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }


                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }







            uint vk = info.vkCode;

            // ---- 일본 IME 관련 키 ----
            bool isImeTriggerKey =
            vk == VK_KANJI ||      // 半角/全角
            vk == VK_NONCONVERT || // 無変換
            vk == VK_CONVERT ||    // 変換
            vk == VK_KANA;       // かな

            if (isImeTriggerKey)
            {


                if (isKeyDown)
                {
                    if (_jpImeKeyPressed)
                        return CallNextHookEx(hookID, nCode, wParam, lParam);

                    _jpImeKeyPressed = true;

                    _ = caretController.NotifyImeToggle();
                }
                else
                {
                    _jpImeKeyPressed = false;
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }



            






            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }











        // 타이핑키 
        private static bool IsTypingKey(Keys key)
        {
            // 문자
            if (key >= Keys.A && key <= Keys.Z)
                return true;

            if (key >= Keys.D0 && key <= Keys.D9)
                return true;

            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return true;

            // 공백/편집
            switch (key)
            {
                case Keys.Space:
                    return true;
            }

            // OEM 키들
            if ((int)key >= (int)Keys.Oem1 && (int)key <= (int)Keys.OemBackslash)
                return true;

            return false;
        }

        private static void TriggerSafe(Func<Task> action)
        {
            _ = Task.Run(action);
        }

        internal static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            Process curProcess = Process.GetCurrentProcess();
            try
            {
                ProcessModule curModule = curProcess.MainModule;
                try
                {
                    return SetWindowsHookEx(
                        WH_KEYBOARD_LL,
                        proc,
                        GetModuleHandle(curModule.ModuleName),
                        0);
                }
                finally
                {
                    curModule?.Dispose();
                }
            }
            finally
            {
                curProcess?.Dispose();
            }
        }

        internal delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}