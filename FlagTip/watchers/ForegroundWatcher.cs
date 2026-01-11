using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.Watchers
{
    internal class ForegroundWatcher : IDisposable
    {
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        private WinEventDelegate _procDelegate;
        private IntPtr _hook;

        private uint _lastPid = 0;
        private IntPtr _lastHwnd = IntPtr.Zero;

        public event Action<IntPtr, uint, string> ForegroundChanged;
        public event Action<uint, uint> ForegroundPidChanged;
        // oldPid, newPid

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

        private void WinEventProc(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero)
                return;

            GetWindowThreadProcessId(hwnd, out uint pid);

            // PID 변경 감지
            if (pid != _lastPid)
            {
                ForegroundPidChanged?.Invoke(_lastPid, pid);
                _lastPid = pid;
            }

            _lastHwnd = hwnd;

            string processName = Process.GetProcessById((int)pid).ProcessName;
            ForegroundChanged?.Invoke(hwnd, pid, processName);
        }

        public void Stop()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
        }

        public void Dispose() => Stop();
    }
}
