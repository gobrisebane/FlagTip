using System;
using System.Runtime.InteropServices;

namespace FlagTip.apps
{
    class NotepadHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        const int EM_GETSEL = 0x00B0;
        const int EM_LINEFROMCHAR = 0x00C9;

        public static int GetCaretLine()
        {
            IntPtr notepadHwnd = FindWindow("Notepad", null);
            if (notepadHwnd == IntPtr.Zero) return -1;

            IntPtr editHwnd = FindWindowEx(notepadHwnd, IntPtr.Zero, "Edit", null);
            if (editHwnd == IntPtr.Zero) return -1;

            // EM_GETSEL 반환값: 하위 16비트 = 시작, 상위 16비트 = 끝
            int sel = SendMessage(editHwnd, EM_GETSEL, 0, 0);
            int start = sel & 0xFFFF;

            // 줄 번호 계산 (0-based → 1-based)
            int lineIndex = SendMessage(editHwnd, EM_LINEFROMCHAR, start, 0);
            return lineIndex + 1;
        }
    }
}