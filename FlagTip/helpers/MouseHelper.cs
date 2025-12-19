using FlagTip.Utils; // 외부 유틸리티 사용 가정
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management; // ManagementObjectSearcher 사용을 위해 필요
using System.Text;
using System.Threading.Tasks;
using System.Windows; // System.Windows.Rect 사용을 위해 필요 (WPF 참조 필요)
using System.Windows.Automation;
using System.Windows.Automation.Text; // TextPattern 사용을 위해 필요
using System.Windows.Forms; // Cursor.Position 사용
using static FlagTip.Utils.NativeMethods; // GetForegroundProcessId, RECT 정의 등이 있다고 가정
using FlagTip.caret;


namespace FlagTip.Helpers
{
    internal class MouseHelper
    {


        internal static bool TryGetCaretFromMouseClick(out RECT rect)
        {
            rect = new RECT();

            try
            {


                var pt = Cursor.Position; // System.Drawing.Point (X, Y)

                //var pt = CaretContext.LastClickPoint;

                //Console.WriteLine(">>>>wahtsapp enter hello");

                RECT r = new RECT
                {
                    left = pt.X,
                    top = pt.Y,

                    //left = CaretContext.LastClickPoint,
                    //top = CaretContext.LastClickPoint,
                    //right = 100,
                    //bottom = 200

                };

                rect = r;
                CaretContext.CaretMouseLock = true;
                return true;


            }
            catch (Exception ex)
            {
                // 로깅 등 필요 시 추가
                Console.WriteLine($"TryGetCaretFromMouseClick Error: {ex.Message}");
                //method = $"Error ({method})";
            }

            return false;
        }
    }
}