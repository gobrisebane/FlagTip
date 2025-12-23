using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Runtime.InteropServices;
using Accessibility;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows;


using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Helpers
{
    internal class MSAAHelper
    {

        internal static bool TryGetCaretFromMSAA(IntPtr hwnd, out RECT rect, out bool visible)
        {
            rect = new RECT();
            visible = false;

            try
            {
                Guid guid = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71");
                int result = NativeMethods.AccessibleObjectFromWindow(hwnd, NativeMethods.OBJID_CARET, ref guid, out object accessibleObject);

                if (result == 0 && accessibleObject is Accessibility.IAccessible accessible)
                {
                    accessible.accLocation(out int left, out int top, out int width, out int height, 0);
                    RECT r = new RECT
                    {
                            left = left,
                            top = top,
                            right = left + width,
                            bottom = top + height

                    };


                    if (CommonUtils.IsRectValid(r) & width > 0)
                    {
                        rect = r;
                        visible = true;
                        return true;
                    }
                }
            }
            catch
            {
                // 예외 무시
            }

            return false;
        }

    }
}
