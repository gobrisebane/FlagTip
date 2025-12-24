using FlagTip.apps;
using FlagTip.Helpers;
using FlagTip.models;
using FlagTip.ui;
using FlagTip.UI;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UIA;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.ui.CursorFlagTip;

using static FlagTip.config.AppList;

namespace FlagTip.caret
{


  



    internal class Caret
    {

        private IndicatorForm _indicatorForm;
        private CursorFlagTip _cursorFlagTip;




        public Caret(IndicatorForm indicatorForm, CursorFlagTip cursorFlagTip)
        {
         
            _indicatorForm = indicatorForm;
            _cursorFlagTip = cursorFlagTip;

        }


   

        public async Task show(int delayMs = 50)
        {


            await Task.Delay(delayMs);



            IntPtr hwnd = GetForegroundWindow();
            string processName = GetProcessName(hwnd);

            bool visible = false;
            RECT rect;
            CaretMethod method = CaretContext.LastMethod;

            rect = new RECT();



            bool contextChanged =
                CaretContext.LastProcessName != processName ||
                CaretContext.LastMethod == CaretMethod.None;


       

            bool isCursorApp = CursorAppList.Contains(processName);


            Console.WriteLine("isCursorApp : " + isCursorApp);


            if (contextChanged)
            {

                Console.WriteLine("A1.CTX CHANGE");


                if (isCursorApp)
                {
                    Console.WriteLine("1. cursorapp start");
                    method = CaretMethod.Cursor;
                    //_cursorFlagTip.Start();
                    Console.WriteLine("1. end?");

                } else {

                    Console.WriteLine("2. cursorapp stop");

                    _cursorFlagTip.Stop();



                    if (processName == "winword" || processName == "devenv")
                    {
                        UIAHelper.TryGetCaretFromUIA(out rect, out visible);
                        method = CaretMethod.UIA;
                    }
                    else if (processName == "explorer")
                    {
                        UIAExplorerHelper.TryGetCaretFromExplorerUIA(out rect, out visible);
                        method = CaretMethod.ExplorerUIA;
                    }
                    else
                    {

                        if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect, out visible))
                        {
                            method = CaretMethod.GUIThreadInfo;
                        }
                        else if (MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect, out visible))
                        {
                            method = CaretMethod.MSAA;
                        }
                        else
                        {
                            rect = new RECT();
                            method = CaretMethod.None;
                        }
                    }


                }

                

            } else {

                Console.WriteLine("A2.CTX NOT CHANGE");



                switch (method)
                {

                    case CaretMethod.MSAA:
                        MSAAHelper.TryGetCaretFromMSAA(hwnd, out rect, out visible);
                        break;
                    case CaretMethod.GUIThreadInfo:
                        GUIThreadHelper.TryGetCaretFromGUIThreadInfo(hwnd, out rect, out visible);
                        break;
                    case CaretMethod.ExplorerUIA:
                        UIAExplorerHelper.TryGetCaretFromExplorerUIA(out rect,out visible);
                        break;
                    case CaretMethod.UIA:
                        UIAHelper.TryGetCaretFromUIA(out rect, out visible);
                        break;
                    case CaretMethod.MouseClick:
                        MouseHelper.TryGetCaretFromMouseClick(out rect, out visible);
                        break;
                    case CaretMethod.Cursor:

                        if (!isCursorApp)
                        {

                            Console.WriteLine("!!! CURSORAPP STOPS..");
                            _cursorFlagTip.Stop();
                        }

                        break;
                    case CaretMethod.None:
                        break;
                }
                
            }




            if (!visible)
            {
                if (processName == "notepad" && method == CaretMethod.GUIThreadInfo)
                {
                    // NOTEPAD가 못잡을때 
                    UIAHelper.TryGetCaretFromUIA(out rect, out visible);
                }
            }







            CaretContext.Position = rect;
            CaretContext.LastMethod = method;
            CaretContext.LastProcessName = processName;
            CaretContext.LastHwnd = hwnd;
            CaretContext.LastUpdated = DateTime.Now;
            CaretContext.Visible = visible;



            /*if (!CommonUtils.IsCaretInEditableArea(hwnd, rect))
            {
                _indicatorForm?.BeginInvoke(new Action(() =>
                    _indicatorForm.HideIndicator()
                 ));
                return;
            }*/


            _indicatorForm?.BeginInvoke(new Action(() =>
                _indicatorForm.SetPosition(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
            ));


            Console.WriteLine($"[{processName}][{visible}] ({method}) Caret: L={rect.left}, T={rect.top}, R={rect.right}, B={rect.bottom}");


        }

        public async Task hide(int delayMs = 0)
        {
            _indicatorForm.HideIndicator();
        }





    }






}
