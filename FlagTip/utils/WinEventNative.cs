using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Utils
{
    internal static class WinEventNative
    {
        public const uint EVENT_OBJECT_CREATE = 0x8000;
        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

        public const int OBJID_WINDOW = 0x00000000;
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        public delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
    }

}
