using FlagTip.Caret;
using FlagTip.Models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;
using static OpenCvSharp.ML.LogisticRegression;

using static FlagTip.Utils.CommonUtils;





namespace FlagTip.Helpers

{
    internal class GUIThreadHelper
    {




        internal static bool TryGetCaretFromGUIThreadInfo(IntPtr hwnd, out RECT rect)
        {
            rect = new RECT();

            try
            {
                uint threadId = NativeMethods.GetWindowThreadProcessId(hwnd, out _);
                GUITHREADINFO guiInfo = new GUITHREADINFO();
                guiInfo.cbSize = Marshal.SizeOf(guiInfo);

                if (NativeMethods.GetGUIThreadInfo(threadId, ref guiInfo))
                {
                    RECT r = guiInfo.rcCaret;
                    POINT pt = new POINT { X = r.left, Y = r.top };
                    if (NativeMethods.ClientToScreen(guiInfo.hwndCaret, ref pt))
                    {
                        int width = r.right - r.left;
                        int height = r.bottom - r.top;

                        r.left = pt.X;
                        r.top = pt.Y;
                        r.right = pt.X + width;
                        r.bottom = pt.Y + height;
                    }


                    if (CommonUtils.IsRectValid(r))
                    {
                        rect = r;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("!!! GUITHREAD ERROR" + ex.Message);
                Console.WriteLine("오류: " + ex.Message);
                return false;


            }

            return false;
        }




    }
}




// 사용시 IsCaretInEditableArea가 가림
/*namespace FlagTip.Helpers
{
    internal static class GUIThreadHelper
    {
        internal static bool TryGetCaretFromGUIThreadInfo(
            IntPtr hwnd,
            out RECT rect)
        {
            rect = default;

            if (hwnd == IntPtr.Zero)
                return false;

            uint threadId =
                NativeMethods.GetWindowThreadProcessId(hwnd, out _);

            GUITHREADINFO info = new GUITHREADINFO
            {
                cbSize = Marshal.SizeOf<GUITHREADINFO>()
            };

            if (!NativeMethods.GetGUIThreadInfo(threadId, ref info))
                return false;

            RECT r = info.rcCaret;

            if (r.left == r.right && r.top == r.bottom)
                return false;

            if (info.hwndCaret != IntPtr.Zero)
            {
                POINT pt = new POINT { X = r.left, Y = r.top };
                NativeMethods.ClientToScreen(info.hwndCaret, ref pt);
                r.left = pt.X;
                r.top = pt.Y;
            }

            rect = r;
            return true;
        }
    }
}*/


/*
namespace FlagTip.Helpers
{
    internal class GUIThreadHelper
    {
        internal static bool TryGetCaretFromGUIThreadInfo(
            IntPtr hwnd,
            out RECT rect)
        {
            rect = default;

            if (hwnd == IntPtr.Zero)
                return false;

            try
            {
                uint threadId =
                    NativeMethods.GetWindowThreadProcessId(hwnd, out _);

                GUITHREADINFO guiInfo = new GUITHREADINFO
                {
                    cbSize = Marshal.SizeOf<GUITHREADINFO>()
                };

                if (!NativeMethods.GetGUIThreadInfo(threadId, ref guiInfo))
                    return false;

                RECT r = guiInfo.rcCaret;

                // rcCaret가 비어있으면 실패
                if (r.left == r.right && r.top == r.bottom)
                    return false;

                int width = r.right - r.left;
                int height = r.bottom - r.top;

                // AHK 스타일 fallback
                if (width <= 0) width = 1;
                if (height <= 0) height = 15;

                // client → screen 변환 (hwndCaret이 있을 때만)
                if (guiInfo.hwndCaret != IntPtr.Zero)
                {
                    POINT pt = new POINT
                    {
                        X = r.left,
                        Y = r.top
                    };

                    if (NativeMethods.ClientToScreen(guiInfo.hwndCaret, ref pt))
                    {
                        r.left = pt.X;
                        r.top = pt.Y;
                    }
                }

                r.right = r.left + width;
                r.bottom = r.top + height;

                if (!CommonUtils.IsRectValid(r))
                    return false;

                rect = r;
                return true;
            }
            catch
            {
                // AHK도 내부적으로 실패 시 그냥 false
                return false;
            }
        }
    }
}
*/




