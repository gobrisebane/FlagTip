using FlagTip.caret;
using FlagTip.hooking;
using FlagTip.models;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using UIA;
using static FlagTip.caret.Caret;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Hooking
{
    internal static class MouseHook
    {


        private static DateTime _mouseDownTime;


        internal static IntPtr MouseHookCallback(
    int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, Caret caret)
        {
            if (nCode >= 0)
            {

                var msg = (MouseMessages)wParam;


                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                if (msg == MouseMessages.WM_LBUTTONDOWN || msg == MouseMessages.WM_RBUTTONDOWN)
                {
                    CaretContext.LastClickPoint = new POINT
                    {
                        X = hookStruct.pt.X,
                        Y = hookStruct.pt.Y
                    };

                }

                if (msg == MouseMessages.WM_LBUTTONDOWN)
                {
                    _mouseDownTime = DateTime.UtcNow;
                }


                // 🔴 여기서만 비동기 처리 진입
                if (msg == MouseMessages.WM_LBUTTONDOWN ||
                    msg == MouseMessages.WM_LBUTTONUP)
                {
                    _ = HandleClickAsync(msg, caret);
                }



            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private static async Task HandleClickAsync(
           MouseMessages msg,
           Caret caret)
        {

            await Task.Delay(50);

            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessName(hwnd);


            bool isWhatsapp =
                processName == "whatsapp" ||
                processName == "whatsapp.root";





            if (isWhatsapp)
            {
                Console.WriteLine("AAAA1111");
                if (msg == MouseMessages.WM_LBUTTONDOWN)
                {
                    await caret.show();
                }
                else if (msg == MouseMessages.WM_LBUTTONUP)
                {
                    double elapsedMs =
                        (DateTime.UtcNow - _mouseDownTime).TotalMilliseconds;

                    if (elapsedMs > 300)
                    {
                        await caret.show();
                    }
                }
            }
            else
            {
                Console.WriteLine("AAAA2222");
                
                if (msg == MouseMessages.WM_LBUTTONDOWN)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        await Task.Delay(50);
                        await caret.show();
                    }
                }
                else if (msg == MouseMessages.WM_LBUTTONUP)
                {
                    await caret.show();
                }
                
            }







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
