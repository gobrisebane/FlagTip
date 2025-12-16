using FlagTip.Helpers;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;
using FlagTip.Models;
using FlagTip.caret;


namespace FlagTip.Tracking
{
    internal class CaretTracker
    {

        private Caret _caret;
        private IndicatorForm _indicatorForm;
        private Thread _thread;

        public CaretTracker(IndicatorForm indicatorForm, Caret caret)
        {
            _caret= caret;
            _indicatorForm = indicatorForm;
            _thread = new Thread(Run) { IsBackground = true };
        }

        public void Start() => _thread.Start();


        private void Run()
        {

            while (true)
            {
                try
                {


                    _caret.show();


                    /*
                    
                    IntPtr hwnd = GetForegroundWindow();
                    string processName = GetProcessNameFromHwnd(hwnd);

                    RECT rect;
                    string method;


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
                    else if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect, out method))
                    {
                        Console.WriteLine("t1 - GUIThreadInfo");
                    }
                    else if (MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect, out method))
                    {
                        Console.WriteLine("t2 - MSAA");
                    }
                    else if (UIAHelper.TryGetCaretFromUIA(out rect, out method))
                    {
                        Console.WriteLine("t3 - UIA");
                    }
                    else
                    {
                        rect = new RECT();
                        method = "None";
                    }


                    _indicatorForm?.BeginInvoke(new Action(() =>
                        _indicatorForm.SetPosition(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
                    ));

                    Console.WriteLine($"[{processName}] ({method}) Caret: L={rect.left}, T={rect.top}, R={rect.right}, B={rect.bottom}");
                    */

                    


                }
                catch { }
                Thread.Sleep(3000);
            }
        }


    }
}
