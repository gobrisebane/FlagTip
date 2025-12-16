using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;


using FlagTip.Models;
using FlagTip.Utils;


namespace FlagTip.Helpers
{
    internal class UIAHelper
    {
        internal static bool TryGetCaretFromUIA(out RECT rect, out string method)
        {
            rect = new RECT();
            method = "None";

            try
            {
                AutomationElement focused = AutomationElement.FocusedElement;
                if (focused != null && focused.TryGetCurrentPattern(TextPattern.Pattern, out object patternObj))
                {
                    var textPattern = (TextPattern)patternObj;
                    TextPatternRange[] selectionRanges = textPattern.GetSelection();

                    if (selectionRanges != null && selectionRanges.Length > 0)
                    {
                        var range = selectionRanges[0];
                        var rects = range.GetBoundingRectangles();

                        if (rects != null && rects.Length > 0)
                        {
                            var r = rects[0];
                            RECT rectUA = new RECT
                            {
                                left = (int)r.Left,
                                top = (int)r.Top,
                                right = (int)r.Right,
                                bottom = (int)r.Bottom
                            };

                            if (CommonUtils.IsRectValid(rectUA))
                            {
                                rect = rectUA;
                                method = "UIA";
                                return true;
                            }
                        }
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
