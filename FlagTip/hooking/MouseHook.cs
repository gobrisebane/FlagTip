using FlagTip.Caret;
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
using static FlagTip.config.AppList;

using static FlagTip.Caret.CaretController;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Hooking
{
    internal static class MouseHook
    {

        private static DateTime _lastWheelTime;
        private static DateTime _mouseDownTime;


        private static bool _isLeftButtonDown;

        private static CancellationTokenSource _holdCts;


        internal static IntPtr MouseHookCallback(
    int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, Caret.CaretController caretController)
        {



            if (nCode >= 0)
            {

                var msg = (MouseMessages)wParam;


                /*
                if (msg == MouseMessages.WM_LBUTTONDOWN || 
                    msg == MouseMessages.WM_MBUTTONDOWN)
                {
                    _mouseDownTime = DateTime.UtcNow;
                    StartHoldLog(caretController);
                }
                else if (msg == MouseMessages.WM_LBUTTONUP || 
                    msg == MouseMessages.WM_MBUTTONUP)
                {
                    StopHoldLog();
                }
                */




                if (msg == MouseMessages.WM_LBUTTONDOWN ||
                    msg == MouseMessages.WM_LBUTTONUP ||
                    msg == MouseMessages.WM_MBUTTONDOWN ||
                    msg == MouseMessages.WM_MBUTTONUP ||
                    msg == MouseMessages.WM_MOUSEWHEEL ||
                    msg == MouseMessages.WM_MOUSEHWHEEL ||
                    msg == MouseMessages.WM_XBUTTONDOWN ||
                    msg == MouseMessages.WM_XBUTTONUP 


                    )
                {
                    _ = HandleClickAsync(msg, caretController);
                }



            }



            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private static async Task HandleClickAsync(
           MouseMessages msg,
           Caret.CaretController caretController)
        {



            if (msg == MouseMessages.WM_MOUSEWHEEL ||
                msg == MouseMessages.WM_MOUSEHWHEEL)
            {

                if ((DateTime.UtcNow - _lastWheelTime).TotalMilliseconds > 100)
                {
                    _lastWheelTime = DateTime.UtcNow;
                    await caretController.SelectModeAfterWheel();
                }

                return;
            }


            if (msg == MouseMessages.WM_LBUTTONDOWN || 
                msg == MouseMessages.WM_MBUTTONDOWN || 
                msg == MouseMessages.WM_XBUTTONDOWN)
            {

                await caretController.SelectModeMultiple();




            }
            else if (msg == MouseMessages.WM_LBUTTONUP || 
                        msg == MouseMessages.WM_MBUTTONUP ||
                        msg == MouseMessages.WM_XBUTTONUP)
            {
                await caretController.SelectMode();
            }
                

        }


        /*

        private static void StartHoldLog(Caret.CaretController caret)
        {
            _isLeftButtonDown = true;

            _holdCts?.Cancel();
            _holdCts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(200, _holdCts.Token);

                    while (_isLeftButtonDown &&
                           !_holdCts.Token.IsCancellationRequested)
                    {

                        await caret.SelectMode();
                        await Task.Delay(150, _holdCts.Token);
                    }
                }
                catch (TaskCanceledException) { }
            });
        }

        private static void StopHoldLog()
        {
            _isLeftButtonDown = false;
            _holdCts?.Cancel();
        }

        */








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

