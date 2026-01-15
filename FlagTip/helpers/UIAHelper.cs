using FlagTip.Caret;
using FlagTip.Models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using UIAutomationClient;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.Utils.CommonUtils;


/*
namespace FlagTip.Helpers
{
    internal class UIAHelper
    {
        internal static bool TryGetCaretFromUIA(out RECT rect)
        {
            // 네이티브 CUIAutomation을 사용하여 관리형 UIA 프록시 문제를 회피합니다.
            rect = new RECT();


            CUIAutomation uia = null;
            IUIAutomationElement element = null;
            IUIAutomationTextPattern textPattern = null;
            IUIAutomationTextRangeArray ranges = null;
            IUIAutomationTextRange range = null;

            try
            {
                uia = new CUIAutomation();
                element = uia.GetFocusedElement();
                if (element == null)
                {
                    return false;
                }

                object patternObj;
                patternObj = element.GetCurrentPattern(10014); // UIA_TextPatternId 값 직접 사용
                textPattern = patternObj as IUIAutomationTextPattern;
                if (textPattern == null)
                {
                    return false;
                }

                ranges = textPattern.GetSelection();
                if (ranges == null)
                {
                    return false;
                }

                int length = 0;
                try
                {
                    // get_Length(out length); 대신 Length 속성을 직접 사용
                    length = ranges.Length;
                }
                catch
                {
                    length = 0;
                }

                if (length == 0)
                {
                    return false;
                }

                // 첫 번째 선택 범위를 사용
                range = ranges.GetElement(0);
                if (range == null)
                {
                    return false;
                }

                // 네이티브는 double[]로 bounding rectangles을 반환 (각 rect는 left, top, width, height)
                double[] rects = null;
                try
                {
                    // GetBoundingRectangles()는 System.Array를 반환하므로 직접 double[]로 할당할 수 없습니다.
                    Array arr = range.GetBoundingRectangles();
                    if (arr != null)
                    {
                        rects = arr as double[];
                        if (rects == null)
                        {
                            // 요소를 double로 변환하여 새 배열 생성
                            rects = new double[arr.Length];
                            for (int i = 0; i < arr.Length; i++)
                            {
                                object val = arr.GetValue(i);
                                rects[i] = Convert.ToDouble(val);
                            }
                        }
                    }
                    else
                    {
                        rects = null;
                    }
                }
                catch
                {
                    rects = null;
                }

                if (rects == null || rects.Length < 4)
                {
                    return false;
                }

                // 첫 번째 rect 값 사용 (left, top, width, height)
                double left = rects[0];
                double top = rects[1];
                double width = rects[2];
                double height = rects[3];

                RECT rectUA = new RECT
                {
                    left = (int)left,
                    top = (int)top,
                    right = (int)(left + width),
                    bottom = (int)(top + height)
                };

                if (CommonUtils.IsRectValid(rectUA))
                {
                    rect = rectUA;
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                // COM 개체 해제
                try { if (range != null) Marshal.ReleaseComObject(range); } catch { }
                try { if (ranges != null) Marshal.ReleaseComObject(ranges); } catch { }
                try { if (textPattern != null) Marshal.ReleaseComObject(textPattern); } catch { }
                try { if (element != null) Marshal.ReleaseComObject(element); } catch { }
                try { if (uia != null) Marshal.ReleaseComObject(uia); } catch { }
            }
        }

    }
}
*/







// 새로운 버전 시험중 v2


namespace FlagTip.Helpers
{
    internal class UIAHelper
    {
        private static CUIAutomation _uia = new CUIAutomation();

        internal static bool TryGetCaretFromUIA(out RECT rect)
        {
            rect = new RECT();

            CUIAutomation uia = _uia;
            if (uia == null)
                return false;

            IUIAutomationElement focused = null;
            IUIAutomationTextPattern textPattern = null;
            IUIAutomationValuePattern valuePattern = null;
            IUIAutomationTextRangeArray ranges = null;
            IUIAutomationTextRange range = null;

            try
            {
                // 1️⃣ Focused element
                focused = uia.GetFocusedElement();
                if (focused == null)
                    return false;

                // 2️⃣ ControlType 필터 (Qt 코드와 동일)
                int controlType = focused.CurrentControlType;
                if (controlType != 50004 && // UIA_EditControlTypeId
                    controlType != 50020 && // UIA_TextControlTypeId
                    controlType != 50030)
                {
                    // ❗ Explorer 주소창은 상위 Pane일 수 있으므로
                    // 여기서 return false 하지 말고 계속 시도하는 것도 가능
                    return false;
                }

                // 3️.ReadOnly 체크 (ValuePattern)
                object vpObj = focused.GetCurrentPattern(10002); // UIA_ValuePatternId
                valuePattern = vpObj as IUIAutomationValuePattern;
                if (valuePattern != null && valuePattern.CurrentIsReadOnly != 0)
                    return false;

                // 4️.TextPattern (Native 방식)
                object tpObj = focused.GetCurrentPattern(10014); // UIA_TextPatternId
                textPattern = tpObj as IUIAutomationTextPattern;
                if (textPattern == null)
                    return false;

                // 5️.Selection (Caret)
                ranges = textPattern.GetSelection();
                if (ranges == null || ranges.Length == 0)
                    return false;

                range = ranges.GetElement(0);
                if (range == null)
                    return false;

                // 6️⃣ Bounding Rectangles
                double[] rects = GetBoundingRects(range);
                if (rects == null || rects.Length < 4)
                {
                    // Qt 코드와 동일한 fallback
                    range.ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Character); // 명확한 네임스페이스 지정
                    rects = GetBoundingRects(range);
                }

                if (rects == null || rects.Length < 4)
                    return false;

                double left = rects[0];
                double top = rects[1];
                double width = rects[2];
                double height = rects[3];

                // width/height == 0 처리
                if (width == 0 && height == 0)
                {
                    range.ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Character); // 명확한 네임스페이스 지정
                    rects = GetBoundingRects(range);
                    if (rects == null || rects.Length < 4)
                        return false;

                    width = rects[2];
                    height = rects[3];
                }

                if (width >= 0 && height >= 0)
                {
                    rect.left = (int)left;
                    rect.top = (int)top;
                    rect.right = (int)(left + width);
                    rect.bottom = (int)(top + height);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log("!!! UIA CATCH ERROR" + ex.Message);
                Console.WriteLine("오류: " + ex.Message);
                return false;
            }
            finally
            {
                // COM 해제 (중요)
                Release(range);
                Release(ranges);
                Release(textPattern);
                Release(valuePattern);
                Release(focused);
            }

            return false;
        }

        private static double[] GetBoundingRects(IUIAutomationTextRange range)
        {
            Array arr = range.GetBoundingRectangles();
            if (arr == null || arr.Length < 4)
                return null;

            double[] rects = new double[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                rects[i] = (double)arr.GetValue(i);

            return rects;
        }

        private static void Release(object obj)
        {
            if (obj != null && Marshal.IsComObject(obj))
                Marshal.ReleaseComObject(obj);
        }
    }
}







// 가장 처음 버전
/*

namespace FlagTip.Helpers
{
    internal class UIAHelper
    {
        internal static bool TryGetCaretFromUIA(out RECT rect)
        {
            rect = default;

            CUIAutomation uia = null;
            IUIAutomationElement element = null;
            IUIAutomationTextPattern textPattern = null;
            IUIAutomationTextRangeArray ranges = null;
            IUIAutomationTextRange range = null;

            try
            {
                uia = new CUIAutomation();
                element = uia.GetFocusedElement();
                if (element == null)
                    return false;

                textPattern = element.GetCurrentPattern(10014) as IUIAutomationTextPattern; // UIA_TextPatternId
                if (textPattern == null)
                    return false;

                ranges = textPattern.GetSelection();
                if (ranges == null || ranges.Length == 0)
                    return false;

                range = ranges.GetElement(0);
                if (range == null)
                    return false;

                var arr = range.GetBoundingRectangles();
                if (arr == null || arr.Length < 4)
                    return false;

                double left = (double)arr.GetValue(0);
                double top = (double)arr.GetValue(1);
                double width = (double)arr.GetValue(2);
                double height = (double)arr.GetValue(3);

                rect = new RECT
                {
                    left = (int)left,
                    top = (int)top,
                    right = (int)(left + width),
                    bottom = (int)(top + height)
                };

                return CommonUtils.IsRectValid(rect);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (range != null) Marshal.ReleaseComObject(range);
                if (ranges != null) Marshal.ReleaseComObject(ranges);
                if (textPattern != null) Marshal.ReleaseComObject(textPattern);
                if (element != null) Marshal.ReleaseComObject(element);
                if (uia != null) Marshal.ReleaseComObject(uia);
            }
        }

    }
}*/

