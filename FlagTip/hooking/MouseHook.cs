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
        private static bool _isLeftButtonDown;
        private static CancellationTokenSource _holdCts;

        private static DateTime _lastClickTime = DateTime.MinValue;
        private static POINT _lastClickPos;


        internal static IntPtr MouseHookCallback(
    int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, Caret.CaretController caretController)
        {

            if (nCode >= 0)
            {

                var msg = (MouseMessages)wParam;


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



                if (msg == MouseMessages.WM_LBUTTONDOWN)
                {
                    MSLLHOOKSTRUCT hookStruct =
                        Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                    if (IsDoubleClick(hookStruct.pt))
                    {
                        _ = HandleDoubleClickAsync(msg,caretController);
                    }
                    
                }






            }



            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private static async Task HandleDoubleClickAsync(
           MouseMessages msg,
            Caret.CaretController caretController)
        {

            if (msg == MouseMessages.WM_LBUTTONDOWN)
            {
                await caretController.SelectDoubleClick();
            }

        }


        private static DateTime _mouseDownTime;
        private const int ClickThresholdMs = 300; // 이 이하면 "짧은 클릭"



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
                _mouseDownTime = DateTime.UtcNow;

                caretController.NotifyCaretMove();

                // original
                //await caretController.MultiSelectMode();


                await caretController.MouseLeftClickMode();


            }


            //original
            else if (msg == MouseMessages.WM_LBUTTONUP || 
                        msg == MouseMessages.WM_MBUTTONUP ||
                        msg == MouseMessages.WM_XBUTTONUP)
            {


                var elapsed = (DateTime.UtcNow - _mouseDownTime).TotalMilliseconds;
                if (elapsed < ClickThresholdMs)
                    return;

                await caretController.SelectMode();

            }







        }





        private static bool IsDoubleClick(POINT currentPos)
        {
            var now = DateTime.UtcNow;

            int maxTime = (int)GetDoubleClickTime();
            int maxDx = System.Windows.Forms.SystemInformation.DoubleClickSize.Width;
            int maxDy = System.Windows.Forms.SystemInformation.DoubleClickSize.Height;

            bool timeOk =
                (now - _lastClickTime).TotalMilliseconds <= maxTime;

            bool posOk =
                Math.Abs(currentPos.X - _lastClickPos.X) <= maxDx &&
                Math.Abs(currentPos.Y - _lastClickPos.Y) <= maxDy;

            _lastClickTime = now;
            _lastClickPos = currentPos;

            return timeOk && posOk;
        }


        [DllImport("user32.dll")]
        static extern uint GetDoubleClickTime();

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

