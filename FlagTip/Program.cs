using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Tracking;
using FlagTip.UI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.Hooking.MouseHook;

using FlagTip.caret;

namespace FlagTip
{
    internal class Program
    {


        private static IntPtr _hookID = IntPtr.Zero;
        private static IndicatorForm indicatorForm;
        private static LowLevelMouseProc _mouseProc; // 대리자 보존

        // -------------------- Main --------------------
        [STAThread]
        static void Main(string[] args)
        {

            indicatorForm = new IndicatorForm();


            var caret = new Caret(indicatorForm);

            // 폼 핸들 강제 생성 (옵션)
            var h = indicatorForm.Handle;


            //var tracker = new CaretTracker(indicatorForm, caret);
            //tracker.Start();




            _mouseProc = (nCode, wParam, lParam) =>
                MouseHookCallback(nCode, wParam, lParam, _hookID, caret);

            _hookID = SetMouseHook(_mouseProc);



            Application.Run(indicatorForm);

            UnhookWindowsHookEx(_hookID);

        }





    }
}