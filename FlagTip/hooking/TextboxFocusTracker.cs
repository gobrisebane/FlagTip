using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIA;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.Hooking
{
    internal static class TextboxFocusTracker
    {
        static WinEventDelegate _proc;
        static IntPtr _hook;

        public static RECT LastTextboxRect;
        public static bool HasRect;

        public static void Start()
        {
            _proc = WinEventCallback;

            _hook = SetWinEventHook(
                EVENT_OBJECT_FOCUS,
                EVENT_OBJECT_LOCATIONCHANGE,
                IntPtr.Zero,
                _proc,
                0,
                0,
                WINEVENT_OUTOFCONTEXT
            );
        }

        public static void Stop()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
        }

        static void WinEventCallback(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime
        )
        {
            // 객체 레벨만
            if (idObject != 0)
                return;

            if (hwnd == IntPtr.Zero)
                return;

            try
            {
                IAccessible acc;
                object child;

                int hr = AccessibleObjectFromEvent(
                    hwnd,
                    idObject,
                    idChild,
                    out acc,
                    out child
                );

                if (hr != 0 || acc == null)
                    return;

                acc.accLocation(out int x, out int y, out int w, out int h, 0);

                if (w <= 0 || h <= 0)
                    return;

                LastTextboxRect = new RECT
                {
                    left = x,
                    top = y,
                    right = x + w,
                    bottom = y + h
                };

                HasRect = true;

                Console.WriteLine(
                    $"[FOCUS] Textbox @ {x},{y} {w}x{h}"
                );
            }
            catch { }
        }
    }
}
