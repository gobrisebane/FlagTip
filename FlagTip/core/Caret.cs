using FlagTip.Helpers;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


//using FlagTip.Models;
using FlagTip.models;



namespace FlagTip.caret
{
    internal class Caret
    {

        private IndicatorForm _indicatorForm;
        internal static CaretMethod LastMethod { get; private set; } = CaretMethod.None;



        public Caret(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;

        }


        public async Task show(int delayMs = 50)
        {

            await Task.Delay(delayMs);  


            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessNameFromHwnd(hwnd);

            RECT rect;
            string method;


            //Console.WriteLine("processName : " + processName);


            if (processName == "winword")
            {
                Console.WriteLine("s1 - word");
                UIAHelper.TryGetCaretFromUIA(out rect);
                LastMethod = CaretMethod.UIA;
            }
            else if (processName == "explorer")
            {
                Console.WriteLine("s2 - explorer");
                UIAExplorerHelper.TryGetCaretFromExplorerUIA(out rect);
                LastMethod = CaretMethod.UIA;
            }
            else if (processName == "whatsapp" || processName == "whatsapp.root")
            {
                Console.WriteLine("s3 - whatsapp..");
                MouseHelper.TryGetCaretFromMouseClick(out rect);
                LastMethod = CaretMethod.MouseClick;

            }
            else if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect))
            {
                Console.WriteLine("t1 - GUIThreadInfo");
                LastMethod = CaretMethod.GUIThreadInfo;

            }

            else if (MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect))
            {
                Console.WriteLine("t2 - MSAA");
                LastMethod = CaretMethod.MSAA;

            }


            /*
            else if (UIAHelper.TryGetCaretFromUIA(out rect))
            {
                Console.WriteLine("t3 - UIA");
                LastMethod = CaretMethod.UIA;
            }
            */


            /*
            else
            {
                Console.WriteLine("tend - NONE");

                rect = new RECT();
                method = "None";
            }
            */




            _indicatorForm?.BeginInvoke(new Action(() =>
                _indicatorForm.SetPosition(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
            ));

            //Console.WriteLine($"[{processName}] ({LastMethod}) Caret: L={rect.left}, T={rect.top}, R={rect.right}, B={rect.bottom}");







        }





    }
}
