using FlagTip.Caret;
using FlagTip.Helpers;
using FlagTip.Hooking;
using FlagTip.Tracking;
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



        // -------------------- Main --------------------
        [STAThread]
        static void Main(string[] args)
        {



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
         

            Application.Run(new MainForm());



            


        }


       






    }
}






