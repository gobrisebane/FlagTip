using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using UIAutomationClient;

using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Helpers
{
    internal class OtherHelper
    {

        internal static bool TryGetRangeFromPoint(out RECT rect, out string method)
        {

            rect = new RECT();
            method = "None";


            try
            {
                // 현재 포커스된 엘리먼트 가져오기
                AutomationElement focusedElement = AutomationElement.FocusedElement;
                if (focusedElement == null) return false;

                // 화면에서 마우스 포인터 위치 가져오기
                POINT pt;
                NativeMethods.GetCursorPos(out pt);

                // TextPattern 가져오기
                if (focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out object patternObj))
                {
                    TextPattern textPattern = (TextPattern)patternObj;

                    Console.WriteLine("a1");

                    // RangeFromPoint 사용
                    TextPatternRange range = textPattern.RangeFromPoint(new System.Windows.Point(pt.X, pt.Y));

                    Console.WriteLine("range : " + range);

                    if (range != null)
                    {
                        var boundingRects = range.GetBoundingRectangles();

                        Console.WriteLine("boundingRects : " + boundingRects[0].Left);

                        if (boundingRects.Length > 0)
                        {
                            var r = boundingRects[0];
                            rect.left = (int)r.Left;
                            rect.top = (int)r.Top;
                            rect.right = (int)r.Right;
                            rect.bottom = (int)r.Bottom;

                            Console.WriteLine("rect.left : " + rect.left);
                            method = "UIA_RangeFromPoint";

                            return true;
                        }
                    }
                }
            }
            catch { }

            return false;

        }

        internal static bool TryGetCaretFromSelectionUIA(out RECT rect, out string method)
        {

            rect = new RECT();
            method = "Explorer";

            try
            {
                // COM UIAutomation 객체
                CUIAutomation uia = new CUIAutomation();

                // 현재 포커스된 요소 가져오기
                IUIAutomationElement element = uia.GetFocusedElement();
                if (element == null)
                    return false;

                // TextPattern 패턴 ID
                //int textPatternId = UIA_PatternIds.UIA_TextPatternId;
                int textPatternId = 10014; // UIA_TextPatternId

                // TextPattern 가져오기
                object patternObj = element.GetCurrentPattern(textPatternId);
                if (patternObj is IUIAutomationTextPattern textPattern)
                {
                    // 선택 영역 가져오기
                    IUIAutomationTextRangeArray ranges = textPattern.GetSelection();
                    if (ranges != null && ranges.Length > 0)
                    {
                        IUIAutomationTextRange range = ranges.GetElement(0);

                        // 선택 영역 좌표 가져오기
                        object rawRects = range.GetBoundingRectangles(); // System.Array


                        if (rawRects is Array arr && arr.Length >= 4)
                        {
                            double left = Convert.ToDouble(arr.GetValue(0));
                            double top = Convert.ToDouble(arr.GetValue(1));
                            double right = Convert.ToDouble(arr.GetValue(2));
                            double bottom = Convert.ToDouble(arr.GetValue(3));

                            Console.WriteLine("left : " + left);

                            //rect.left = (int)left;
                            rect.left = (int)(left + right);
                            rect.top = (int)top;
                            rect.right = (int)right;
                            rect.bottom = (int)bottom;

                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;




        }


        internal static bool TryGetCaretFromExplorerUIABk(out RECT rect, out string method)
        {
            rect = new RECT();
            method = "Explorer";

            try
            {
                CUIAutomation uia = new CUIAutomation();

                IUIAutomationElement element = uia.GetFocusedElement();
                if (element == null)
                    return false;


                int hasFocusInt = element.CurrentHasKeyboardFocus;
                bool hasFocus = hasFocusInt != 0;
                Console.WriteLine("Explorer 주소창 포커스 여부: " + hasFocus);



                int textPatternId = 10014; // UIA_TextPatternId
                object patternObj = element.GetCurrentPattern(textPatternId);
                if (patternObj is IUIAutomationTextPattern textPattern)
                {
                    // 선택 영역 가져오기
                    IUIAutomationTextRangeArray ranges = textPattern.GetSelection();
                    if (ranges != null && ranges.Length > 0)
                    {
                        IUIAutomationTextRange range = ranges.GetElement(0);

                        // 선택 영역 좌표 가져오기
                        object rawRects = range.GetBoundingRectangles(); // System.Array


                        if (rawRects is Array arr && arr.Length >= 4)
                        {
                            double left = Convert.ToDouble(arr.GetValue(0));
                            double top = Convert.ToDouble(arr.GetValue(1));
                            double right = Convert.ToDouble(arr.GetValue(2));
                            double bottom = Convert.ToDouble(arr.GetValue(3));

                            rect.left = (int)left;
                            rect.top = (int)top;
                            rect.right = (int)right;
                            rect.bottom = (int)bottom;

                            NativeMethods.prevCaretLocation.left = rect.left;
                            NativeMethods.prevCaretLocation.top = rect.top;
                            NativeMethods.prevCaretLocation.right = rect.right;
                            NativeMethods.prevCaretLocation.bottom = rect.bottom;

                            //prevWidth = rect.left + rect.right;


                            Console.WriteLine("wwww1111");
                            Console.WriteLine("prevCaretLocation.left : " + NativeMethods.prevCaretLocation.left);

                            return true;

                        }
                        else
                        {

                            Console.WriteLine("wwww2222");
                            Console.WriteLine("prevCaretLocation.left : " + NativeMethods.prevCaretLocation.left);

                            //rect.left = prevWidth;
                            rect.left = NativeMethods.prevCaretLocation.left;
                            rect.top = NativeMethods.prevCaretLocation.top;
                            rect.right = NativeMethods.prevCaretLocation.right;
                            rect.bottom = NativeMethods.prevCaretLocation.bottom;

                            return true;

                        }
                    }


                }
            }
            catch
            {
                return false;
            }

            return false;

        }


    }
}
