using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Models;
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

        // -------------------- Main --------------------
        [STAThread]
        static void Main(string[] args)
        {

            indicatorForm = new IndicatorForm();




            var caret = new Caret(indicatorForm);

            //caret.show();


            //var tracker = new CaretTracker(indicatorForm);
            //tracker.Start();




            _hookID = SetMouseHook((nCode, wParam, lParam) =>
                MouseHookCallback(nCode, wParam, lParam, _hookID, caret)
            );




            Application.Run(indicatorForm);


            UnhookWindowsHookEx(_hookID);

        }





    }
}