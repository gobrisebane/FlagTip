using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.watchers
{
    internal class ForegroundWatcher : IDisposable
    {
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        private WinEventDelegate _procDelegate;
        private IntPtr _hook;

        public event Action<IntPtr, string> ForegroundChanged;

        public void Start()
        {
            _procDelegate = WinEventProc;
            _hook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND,
                EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero,
                _procDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT);
        }

        public void Stop()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
        }

        private void WinEventProc(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero) return;

            GetWindowThreadProcessId(hwnd, out uint pid);
            string processName = Process.GetProcessById((int)pid).ProcessName;

            ForegroundChanged?.Invoke(hwnd, processName);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
