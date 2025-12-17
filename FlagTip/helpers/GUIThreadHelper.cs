using FlagTip.Utils;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



using static FlagTip.Utils.NativeMethods;



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
            catch
            {
                // 예외 무시, 실패시 false 반환

            }

            return false;
        }


    }
}
