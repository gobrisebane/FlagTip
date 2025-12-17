using FlagTip.Helpers;
using FlagTip.Models;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.caret
{
    internal class Caret
    {



        /*

        private IndicatorForm _indicatorForm;

        public Caret(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;
        }
        */

        private IndicatorForm _indicatorForm;


        public Caret(IndicatorForm indicatorForm)
        {
            //Console.WriteLine("caret start hello..");
            _indicatorForm = indicatorForm;

            //_indicatorForm = new IndicatorForm();

        }




        public async Task show(int delayMs = 50)
        {

            await Task.Delay(delayMs);  





            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessNameFromHwnd(hwnd);

            RECT rect;
            string method;


            Console.WriteLine("processName : " + processName);


            if (processName == "winword")
            {
                Console.WriteLine("s1 - word");
                UIAHelper.TryGetCaretFromUIA(out rect, out method);
            }
            else if (processName == "explorer")
            {
                Console.WriteLine("s2 - explorer");
                TestHelper.TryGetCaretFromExplorerUIA(out rect, out method);
            }
            else if (processName == "whatsapp" || processName == "whatsapp.root")
            {
                Console.WriteLine("s3 - whatsapp..");
                MouseHelper.TryGetCaretFromMouseClick(out rect, out method);
            }
            else if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect, out method))
            {
                Console.WriteLine("t1 - GUIThreadInfo");
            }

            else if (MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect, out method))
            {
                Console.WriteLine("t2 - MSAA");
            }


            /*
            else if (UIAHelper.TryGetCaretFromUIA(out rect, out method))
            {
                Console.WriteLine("t3 - UIA");
            }
            */
            
            else
            {
                Console.WriteLine("tend - NONE");

                rect = new RECT();
                method = "None";
            }




            _indicatorForm?.BeginInvoke(new Action(() =>
                _indicatorForm.SetPosition(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
            ));

            Console.WriteLine($"[{processName}] ({method}) Caret: L={rect.left}, T={rect.top}, R={rect.right}, B={rect.bottom}");







        }





    }
}
