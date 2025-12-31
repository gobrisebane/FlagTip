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

        private static long _lastTriggerTick;
        private const int TRIGGER_INTERVAL_MS = 40;
        private static bool _hangulKeyPressed = false;
        private static bool _capsLockKeyPressed = false;


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
                        _ctrlUp= up;
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

                    case Keys.LWin:
                    case Keys.RWin:
                        _winDown = down;
                        break;
                }
            }




            //if (isKeyDown && key == Keys.HangulMode)
            //{
            //    caretController.NotifyImeToggle();
            //    return CallNextHookEx(hookID, nCode, wParam, lParam);
            //}

            if (key == Keys.HangulMode)
            {
                if (isKeyDown)
                {
                    if (_hangulKeyPressed)
                    {
                        return CallNextHookEx(hookID, nCode, wParam, lParam);
                    }

                    _hangulKeyPressed = true;
                    caretController.NotifyImeToggle();
                }
                else
                {
                    _hangulKeyPressed = false;
                    caretController.NotifyImeToggle();
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }




            if (key == Keys.CapsLock)
            {
                if (isKeyDown)
                {
                    if (_capsLockKeyPressed)
                    {
                        return CallNextHookEx(hookID, nCode, wParam, lParam);
                    }
                    _capsLockKeyPressed = true;
                    caretController.NotifyCapsLockToggle();
                }
                else
                {
                    _capsLockKeyPressed = false;
                    caretController.NotifyCapsLockToggle();
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




            // ---- 단축키 ----
            if (_ctrlDown && key == Keys.Y)
            {
                TriggerSafe(caretController.OnKeyTest);
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }

            if (_ctrlDown && key == Keys.A)
            {
                caretController.NotifySelectAll();
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }

           


            if (_winDown)
            {
                TriggerSafe(() => caretController.MultiSelectMode());
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }


            if (CaretKeys.Contains(key))
            {
                //return
                caretController.NotifyCaretMove();
                TriggerSafe(() => caretController.OnKeyChangedAsync());

                if (key == Keys.Enter)
                {
                    TriggerSafe(() => caretController.MultiSelectModeBrowser(5));
                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }


                  // ctrl/alt 시에 적용 (_shiftDown 은 타이핑때문에 제외)
            if (_ctrlDown || _altDown)
            {
                //return
                TriggerSafe(() => caretController.OnKeyChangedAsync());
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }




            



            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }






        /*    private static HashSet<Keys> _pressedKeys = new();

            private static bool IsKeyHold(Keys key, bool isKeyDown)
            {
                if (isKeyDown)
                {
                    if (_pressedKeys.Contains(key))
                        return true;   // 🔁 홀드 반복

                    _pressedKeys.Add(key);
                    return false;      // ✅ 첫 눌림
                }
                else
                {
                    _pressedKeys.Remove(key);
                    return false;
                }
            }

    */



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