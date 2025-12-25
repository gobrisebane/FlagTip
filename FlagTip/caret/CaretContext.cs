using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Caret
{
    internal static class CaretContext
    {
        internal static CaretMethod LastMethod { get; set; } = CaretMethod.None;

        internal static CaretInitator LastCaretInitator { get; set; } = CaretInitator.None;

        internal static string LastProcessName { get; set; } = string.Empty;

        internal static string LastClassName { get; set; } = string.Empty;

        internal static IntPtr LastHwnd { get; set; } = IntPtr.Zero;


        

        internal static DateTime LastUpdated { get; set; } = DateTime.MinValue;

        public static bool IsWebView2 { get; private set; }

        public static bool CaretMouseLock { get; set; } = false;


        internal static RECT LastRect { get; set; }




        internal static void Reset()
        {
            LastMethod = CaretMethod.None;
            LastProcessName = string.Empty;
            LastClassName = string.Empty;
            LastHwnd = IntPtr.Zero;
            LastUpdated = DateTime.MinValue;
        }




    }
}
