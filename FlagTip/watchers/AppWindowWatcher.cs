using FlagTip.Utils;
using System;


namespace FlagTip.Watchers
{
    public class AppWindowWatcher : IDisposable
    {
        private IntPtr _hookCreate;
        private IntPtr _hookForeground;

        private WinEventNative.WinEventDelegate _proc;

        public event Action<IntPtr> WindowCreated;
        public event Action<IntPtr> ForegroundChanged;

        public void Start()
        {
            _proc = WinEventCallback; // GC 방지 ⭐⭐⭐

            _hookCreate = WinEventNative.SetWinEventHook(
                WinEventNative.EVENT_OBJECT_CREATE,
                WinEventNative.EVENT_OBJECT_CREATE,
                IntPtr.Zero,
                _proc,
                0,
                0,
                WinEventNative.WINEVENT_OUTOFCONTEXT
            );

            _hookForeground = WinEventNative.SetWinEventHook(
                WinEventNative.EVENT_SYSTEM_FOREGROUND,
                WinEventNative.EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero,
                _proc,
                0,
                0,
                WinEventNative.WINEVENT_OUTOFCONTEXT
            );
        }

        private void WinEventCallback(
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

            if (idObject != WinEventNative.OBJID_WINDOW)
                return;

            switch (eventType)
            {
                case WinEventNative.EVENT_OBJECT_CREATE:
                    WindowCreated?.Invoke(hwnd);
                    break;

                case WinEventNative.EVENT_SYSTEM_FOREGROUND:
                    ForegroundChanged?.Invoke(hwnd);
                    break;
            }
        }

        public void Dispose()
        {
            if (_hookCreate != IntPtr.Zero)
                WinEventNative.UnhookWinEvent(_hookCreate);

            if (_hookForeground != IntPtr.Zero)
                WinEventNative.UnhookWinEvent(_hookForeground);
        }
    }

}