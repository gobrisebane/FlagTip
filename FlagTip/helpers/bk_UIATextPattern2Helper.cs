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
    internal class bk_UIAHelper
    {
        internal static bool TryGetCaretFromUIA(out RECT rect, out string method)
        {
            rect = new RECT();
            method = "None";

            try
            {
                /*
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

                            //double width = r.Width;
                            //int width = (int)r.Right - (int)r.Left;
                            //Console.WriteLine("!!!!!!!!!33333 : " + range);

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
                */


                /*
                AutomationElement focused = AutomationElement.FocusedElement;
                if (focused == null)
                    return false;

                if (!focused.TryGetCurrentPattern(TextPattern.Pattern, out object patternObj))
                    return false;

                var textPattern = (TextPattern)patternObj;
                var selections = textPattern.GetSelection();

                if (selections == null || selections.Length == 0)
                    return false; // WebView2 정상 케이스

                var range = selections[selections.Length - 1];
                var rects = range.GetBoundingRectangles();

                if (rects == null || rects.Length == 0)
                    return false;

                var r = rects[rects.Length - 1];

                int caretX = (int)r.Right;
                int caretY = (int)r.Top;
                int caretHeight = (int)(r.Bottom - r.Top);

                RECT caretRect = new RECT
                {
                    left = caretX,
                    top = caretY,
                    right = caretX + 2,
                    bottom = caretY + caretHeight
                };

                if (!CommonUtils.IsRectValid(caretRect))
                    return false;

                rect = caretRect;
                method = "UIA_SelectionAsCaret";
                return true;

                */


                AutomationElement focused = AutomationElement.FocusedElement;
                if (focused == null)
                    return false;

                AutomationPattern textPattern2 =
                    AutomationPattern.LookupById(10024);

                if (!focused.TryGetCurrentPattern(textPattern2, out object patternObj))
                    return false;

                dynamic tp2 = patternObj;

                var caretRange = tp2.GetCaretRange(out bool isActive);
                if (!isActive || caretRange == null)
                    return false;

                double[] rects = caretRange.GetBoundingRectangles();
                if (rects == null || rects.Length < 4)
                    return false;

                rect = new RECT
                {
                    left = (int)rects[0],
                    top = (int)rects[1],
                    right = (int)(rects[0] + rects[2]),
                    bottom = (int)(rects[1] + rects[3])
                };

                method = "UIA_TextPattern2(dynamic)";
                return true;




            }
            catch
            {
             
                // 예외 무시
            }

            return false;
        }

    }
}
