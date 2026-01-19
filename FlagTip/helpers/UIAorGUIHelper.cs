using FlagTip.Caret;
using FlagTip.Models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms; // Cursor.Position 사용
using UIAutomationClient;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.Utils.CommonUtils;



namespace FlagTip.Helpers
{
    internal class UIAorGUIHelper
    {

        internal static bool TryGetCaretFromUIAorGUI(IntPtr hwnd, out RECT rect, String processName)
        {
            
            //caretLocation = new RECT();
            rect = new RECT();

            try
            {

                if (UIAHelper.TryGetCaretFromUIA(out rect) 
                    && CommonUtils.IsRectValid(rect))
                {
                    return true;
                }

                if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect, processName) 
                    && CommonUtils.IsRectValid(rect))
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {

                Log("!!! UIAorGUI CATCH ERROR" + ex.Message);
                Console.WriteLine("오류: " + ex.Message);
                return false;
            }

        }


    }
}
