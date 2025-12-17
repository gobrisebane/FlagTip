using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.caret.Caret;
using FlagTip.caret;


namespace FlagTip.Hooking
{
    internal static class MouseHook
    {

        internal static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, Caret caret)
        {

            if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
            {

                Console.WriteLine("Left mouse button clicked!");



                //caret.show();


                _ = Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        caret.show();
                        await Task.Delay(70); // 30~60ms 권장
                    }
                });


            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        internal static IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

    }
}
