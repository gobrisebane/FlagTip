using FlagTip.Helpers;
using FlagTip.Ime;

//using FlagTip.Models;
using FlagTip.models;
using FlagTip.UI;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UIA;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;



namespace FlagTip.caret
{


  



    internal class Caret
    {

        private IndicatorForm _indicatorForm;




        public Caret(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;

        }


        public async Task show(int delayMs = 50)
        {




          






            await Task.Delay(delayMs);  


            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessName(hwnd);

            RECT rect;
            CaretMethod method = CaretContext.LastMethod;

            rect = new RECT();



            bool contextChanged =
            CaretContext.LastProcessName != processName ||
            CaretContext.LastMethod == CaretMethod.None;



            if (contextChanged)
            {


                if (processName == "whatsapp" || processName == "whatsapp.root")
                {

                    MouseHelper.TryGetCaretFromMouseClick(out rect);
                    method = CaretMethod.MouseClick;
                }
                else if(processName == "winword" || processName == "devenv")
                {
                    UIAHelper.TryGetCaretFromUIA(out rect);
                    method = CaretMethod.UIA;
                }
                else if(processName == "explorer")
                {
                    UIAExplorerHelper.TryGetCaretFromExplorerUIA(out rect);
                    method = CaretMethod.ExplorerUIA;
                }

                else {

                    if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect))
                    {
                        method = CaretMethod.GUIThreadInfo;
                    }
                    else if (MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect))
                    {
                        method = CaretMethod.MSAA;
                    }
                    else if (UIAHelper.TryGetCaretFromUIA(out rect))
                    {
                        // 크롬에서 읽는문제때문에 단독 제외
                        method = CaretMethod.UIA;
                    }
                    

                    else
                    {
                        rect = new RECT();
                        method = CaretMethod.None;
                    }


                }
            }




            switch (method)
            {

                case CaretMethod.MSAA:
                    MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect);
                    break;
                case CaretMethod.GUIThreadInfo:
                    GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect);
                    break;
                case CaretMethod.ExplorerUIA:
                    UIAExplorerHelper.TryGetCaretFromExplorerUIA(out rect);
                    break;
                case CaretMethod.UIA:
                    UIAHelper.TryGetCaretFromUIA(out rect);
                    break;
                case CaretMethod.MouseClick:
                    MouseHelper.TryGetCaretFromMouseClick(out rect);
                    break;
                case CaretMethod.None:
                    break;
            }


            if (!CommonUtils.IsCaretInEditableArea(hwnd, rect))
            {
                _indicatorForm.HideIndicator();
                return;
            }

            



            CaretContext.LastMethod = method;
            CaretContext.LastProcessName = processName;
            CaretContext.LastHwnd = hwnd;
            CaretContext.LastUpdated = DateTime.Now;


            _indicatorForm?.BeginInvoke(new Action(() =>
                _indicatorForm.SetPosition(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
            ));




            Console.WriteLine(
                GetImeState()
            );


            Console.WriteLine($"[{processName}] ({method}) Caret: L={rect.left}, T={rect.top}, R={rect.right}, B={rect.bottom}");


        }



    


    }






}
