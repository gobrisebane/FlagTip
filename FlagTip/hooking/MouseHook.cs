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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using UIA;
using static FlagTip.caret.Caret;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Hooking
{
    internal static class MouseHook
    {

        private static DateTime _lastWheelTime;
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
                    msg == MouseMessages.WM_LBUTTONUP 
                    
                    
                    //|| msg == MouseMessages.WM_MOUSEWHEEL 
                    //|| msg == MouseMessages.WM_MOUSEHWHEEL

                    )
                {

                    _ = HandleClickAsync(msg, caret);
                }


                else if (msg == MouseMessages.WM_MOUSEWHEEL ||
                         msg == MouseMessages.WM_MOUSEHWHEEL)
                {




                    //_wheelEnterCts?.Cancel();
                    //_wheelEnterCts = new CancellationTokenSource();
                    //var token = _wheelEnterCts.Token;

                    //_ = Task.Run(async () =>
                    //{
                    //    try
                    //    {
                    //        await Task.Delay(150, token); // ← 스크롤 끝날 때까지 대기

                    //        _ = HandleClickAsync(msg, caret); // ← 딱 1번


                    //    }
                    //    catch (TaskCanceledException) { }
                    //});

                    //Console.WriteLine("hello");

                    _ = HandleClickAsync(msg, caret); // ← 딱 1번


                    return CallNextHookEx(hookID, nCode, wParam, lParam);


                }




            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private static async Task HandleClickAsync(
           MouseMessages msg,
           Caret caret)
        {



            if (msg == MouseMessages.WM_MOUSEWHEEL ||
                msg == MouseMessages.WM_MOUSEHWHEEL)
            {


                if ((DateTime.UtcNow - _lastWheelTime).TotalMilliseconds > 100)
                {
                    _lastWheelTime = DateTime.UtcNow;
                    await caret.show();
                }

                return;
            }



            await Task.Delay(50);
            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessName(hwnd);
            
            bool isWhatsapp =
                processName == "whatsapp" ||
                processName == "whatsapp.root";


            if (isWhatsapp)
            {
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

