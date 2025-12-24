using FlagTip.caret;
using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Tracking;
using FlagTip.ui;
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            indicatorForm = new IndicatorForm();



            //var cursorFlagTip = new CursorFlagTip(indicatorForm);


            var caret = new Caret(indicatorForm, cursorFlagTip);

          


            _mouseProc = (nCode, wParam, lParam) =>
                MouseHookCallback(nCode, wParam, lParam, _hookID, caret);

            _hookID = SetMouseHook(_mouseProc);




            var tracker = new CaretTracker(caret);
            tracker.Start();






            Application.Run(indicatorForm);





            UnhookWindowsHookEx(_hookID);

        }







    }
}






