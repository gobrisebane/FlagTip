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
    internal class UIAExplorerHelper
    {


        internal static bool TryGetCaretFromUIAExplorer(IntPtr hwnd,out RECT caretLocation)
        {
            caretLocation = new RECT();

            try
            {

                if (UIAorGUIHelper.TryGetCaretFromUIAorGUI(hwnd, out caretLocation)
                    && CommonUtils.IsRectValid(caretLocation))
                {
                    return true;

                } else {

                    var uia = new CUIAutomation();
                    var element = uia.GetFocusedElement();
                    if (element == null) return false;

                    int controlType = element.CurrentControlType;
                    string className = ExplorerInfo.GetForegroundWindowClassName();

                    // Explorer가 아닐 경우 조기 종료
                    if (!string.Equals(className, "CabinetWClass", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(className, "Progman", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    var parent = uia.ControlViewWalker.GetParentElement(element);
                    if (parent == null) return false;

                    var boundingRect = element.CurrentBoundingRectangle;

                    if (parent.CurrentControlType == 50026)
                    {
                        if (controlType == 50004)
                        {
                            caretLocation.left = boundingRect.left + 11;
                            caretLocation.top = boundingRect.top + 6;
                            caretLocation.right = boundingRect.right;
                            caretLocation.bottom = boundingRect.bottom;
                        }
                    }
                    else if (parent.CurrentControlType == 50008)
                    {
                        if (controlType == 50004 || controlType == 50030)
                        {
                            caretLocation.left = boundingRect.left + 1;
                            caretLocation.top = boundingRect.top;
                            caretLocation.right = boundingRect.right;
                            caretLocation.bottom = boundingRect.bottom;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                Log("!!! UIAEXPLORER ERROR" + ex.Message);
                Console.WriteLine("오류: " + ex.Message);
                return false;
            }

        }


    }
}
