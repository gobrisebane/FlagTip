using Accessibility;
using FlagTip.Caret;
using FlagTip.models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Helpers
{
    internal class MSAAHelper
    {

        
        internal static bool TryGetCaretFromMSAA(IntPtr hwnd, out RECT rect)
        {
            rect = new RECT();

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




        private const uint OBJID_CLIENT = 0xFFFFFFFC;

        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(
            IntPtr hwnd,
            uint dwObjectID,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppvObject);

        internal static IAccessible GetAccessibleFromWindow(IntPtr hwnd)
        {
            Guid iid = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71"); // IID_IAccessible

            int hr = AccessibleObjectFromWindow(
                hwnd,
                OBJID_CLIENT,
                ref iid,
                out object accObj);

            if (hr >= 0 && accObj is IAccessible acc)
                return acc;

            return null;
        }

        [ComImport]
        [Guid("24FD2FFB-3AAD-4A08-8335-A3AD89C0FB4B")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAccessibleText
        {
            void get_nCharacters(out int nCharacters);
            void get_caretOffset(out int caretOffset);
            void get_text(int startOffset, int endOffset,
                [MarshalAs(UnmanagedType.BStr)] out string text);
        }


    }
}
