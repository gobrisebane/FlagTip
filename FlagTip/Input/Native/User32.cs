using System;
using System.Runtime.InteropServices;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Input.Native
{
    internal static class User32
    {
        // 🔹 GetGUIThreadInfo
        [DllImport("user32.dll")]
        internal static extern bool GetGUIThreadInfo(
            int idThread,
            ref GUITHREADINFO lpgui);


        internal const int SMTO_NORMAL = 0x0000;
        internal const int SMTO_ABORTIFHUNG = 0x0002;
        internal const int SMTO_ERRORONEXIT = 0x0020;
        internal const int WM_IME_CONTROL = 0x283;


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            IntPtr lParam,
            int flags,
            int timeout,
            out IntPtr lpdwResult);
    }



    
}