using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.caret
{
    internal static class CaretContext
    {
        internal static CaretMethod LastMethod { get; set; } = CaretMethod.None;

        internal static CaretInitator LastCaretInitator { get; set; } = CaretInitator.None;

        internal static string LastProcessName { get; set; } = string.Empty;

        internal static IntPtr LastHwnd { get; set; } = IntPtr.Zero;

        internal static DateTime LastUpdated { get; set; } = DateTime.MinValue;

        public static bool IsWebView2 { get; private set; }

        public static bool CaretMouseLock { get; set; } = false;


        public static POINT LastClickPoint;

        public static RECT Position { get; set; }

        public static bool Visible { get; set; }



        /// <summary>상태 초기화</summary>
        internal static void Reset()
        {
            LastMethod = CaretMethod.None;
            LastProcessName = string.Empty;
            LastHwnd = IntPtr.Zero;
            LastUpdated = DateTime.MinValue;
        }



        static readonly HashSet<string> WebView2ProcessNames =
       new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       {
            "whatsapp.exe",
            "whatsapp",
            "whatsapp.root"
       };

        public static void Update()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                IsWebView2 = false;
                return;
            }

            LastProcessName = GetProcessName(hwnd);

            IsWebView2 = WebView2ProcessNames.Contains(LastProcessName);
        }





    }
}
