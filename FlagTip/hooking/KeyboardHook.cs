using FlagTip.Caret;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.Hooking
{
    internal static class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int VK_CONTROL = 0x11; 

        internal static IntPtr KeyboardHookCallback(
            int nCode,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr hookID,
            CaretController caretController)
        {
            if (nCode >= 0 &&
                (wParam == (IntPtr)WM_KEYDOWN ||
                 wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                var key = (Keys)info.vkCode;


                bool isShiftDown =
            (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;

                bool isAltDown =
           (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

                bool isCtrlDown =
            (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;




                /*if (key == Keys.H && isCtrlDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                if (key == Keys.F && isCtrlDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                if (key == Keys.F && isCtrlDown && isShiftDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }*/

                if (key == Keys.R && isCtrlDown)
                {
                    _ = caretController.OnKeyTest();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }


                if (isCtrlDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                if (isShiftDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                if (isAltDown)
                {
                    _ = caretController.OnKeyChangedAsync();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }


                /*
                                if (isCtrlDown && isShiftDown)
                                {
                                    _ = caretController.OnKeyChangedAsync(); 
                                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                                }*/


                switch (key)
                {
                    case Keys.Enter:
                    case Keys.Back:
                    case Keys.Delete:
                    case Keys.Home:   
                    case Keys.End:    
                    case Keys.F2:
                    case Keys.PageUp: 
                    case Keys.PageDown:
                        _ = caretController.OnKeyChangedAsync();
                        break;

                    case Keys.Tab:
                        if (isShiftDown)
                        {
                            _ = caretController.OnKeyChangedAsync();
                        }
                        else
                        {
                            _ = caretController.OnKeyChangedAsync();
                        }
                        break;
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        internal static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    proc,
                    GetModuleHandle(curModule.ModuleName),
                    0);
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

        private const int VK_SHIFT = 0x10;
        private const int VK_MENU = 0x12; 

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);
    }
}