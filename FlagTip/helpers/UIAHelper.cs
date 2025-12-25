using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;

using FlagTip.Utils;
using static FlagTip.Utils.NativeMethods;

using UIAutomationClient;
using System.Runtime.InteropServices;


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
