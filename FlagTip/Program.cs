using FlagTip.caret;
using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Ime;
using FlagTip.Tracking;
using FlagTip.UI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using static FlagTip.Hooking.MouseHook;
using static FlagTip.Utils.NativeMethods;



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

          




            _mouseProc = (nCode, wParam, lParam) =>
                MouseHookCallback(nCode, wParam, lParam, _hookID, caret);

            _hookID = SetMouseHook(_mouseProc);

            Application.Run(indicatorForm);

            UnhookWindowsHookEx(_hookID);

        }







    }
}






