using FlagTip.Caret;
using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Tracking;
using FlagTip.Tray;
using FlagTip.UI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static FlagTip.Hooking.MouseHook;
using static FlagTip.Utils.NativeMethods;



namespace FlagTip
{
    internal class Program
    {

        [DllImport("user32.dll")]
        static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2
            = new IntPtr(-4);


        // -------------------- Main --------------------
        [STAThread]
        static void Main(string[] args)
        {


            Thread.Sleep(3000);

            //WaitForExplorer();

            
            SetProcessDpiAwarenessContext(
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());



            


        }



        static void WaitForExplorer()
        {
            while (Process.GetProcessesByName("explorer").Length == 0)
            {
                Thread.Sleep(500);
            }
        }








    }
}






