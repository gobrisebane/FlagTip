using FlagTip.caret;
using FlagTip.models;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.caret.Caret;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Hooking
{
    internal static class MouseHook
    {

        internal static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, Caret caret)
        {

            if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
            {

                Console.WriteLine(">> Left mouse button clicked!");
                //Console.WriteLine("Caret.LastMethod : " + Caret.LastMethod);




                _ = Task.Run(async () =>
                {

/*
                    int loopCount =
                Caret.LastMethod == CaretMethod.MouseClick ? 1 : 3;

                    for (int i = 0; i < loopCount; i++)
                    {
                        await caret.show();
                        await Task.Delay(70); // 30~60ms 권장
                    }
*/



                    await caret.show();

                    int loopCount =
                Caret.LastMethod == CaretMethod.MouseClick ? 0 : 2;

                    for (int i = 0; i < loopCount; i++)
                    {
                        await Task.Delay(70); // 30~60ms 권장
                        await caret.show();
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
