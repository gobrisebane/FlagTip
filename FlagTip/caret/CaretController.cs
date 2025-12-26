using Accessibility;
using FlagTip.apps;
using FlagTip.helpers;
using FlagTip.Helpers;
using FlagTip.models;
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
using System.Windows.Automation;
using System.Windows.Forms;
using UIA;
using UIAutomationClient;
using static FlagTip.config.AppList;
using static FlagTip.Helpers.MSAAHelper;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;
using static System.Collections.Specialized.BitVector32;
using UIAutomationClient;

namespace FlagTip.Caret
{






    internal class CaretController
    {

        private readonly SemaphoreSlim _selectLock = new SemaphoreSlim(1, 1);



        private IndicatorForm _indicatorForm;

        private CursorHelper _cursorHelper;
        private CaretMethod _method;
        private IntPtr _hwnd;
        private string _processName;
        private string _className;
        private RECT _rect;




        public CaretController(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;
            _cursorHelper = new CursorHelper(_indicatorForm);
        }




        
        public async Task OnKeyChangedAsync()
        {
            Console.WriteLine("winkey");
            await SelectMode();
        }


        public async Task OnKeyTest()
        {

        

        }

        public async Task SelectDoubleClick()
        {
            await SelectMode(10);
        }















        public async Task SelectModeMultiple()
        {
            await Task.Delay(50);
            await SelectMode();

           for (int i = 0; i < 2; i++)
            {
                if (IsProcessCursorApp())
                    break;

                await Task.Delay(80);
                await SelectMode();
            }


        }


        public async Task SelectModeAfterWheel()
        {
            await SelectMode();
            await Task.Delay(200);
            await SelectMode();
        }

   

        public async Task SelectMode(int delayMs = 50)
        {

            if (!await _selectLock.WaitAsync(0))
                return;

            try
            {
                _hwnd = GetForegroundWindow();
                _processName = GetProcessName(_hwnd);
                _method = CaretContext.LastMethod;
                _rect = CaretContext.LastRect;

                //_rect = new RECT();
                StringBuilder classNameBuilder = new StringBuilder(256);
                GetClassName(_hwnd, classNameBuilder, classNameBuilder.Capacity);
                _className = classNameBuilder.ToString();



              

                bool isCursorApp = CursorAppList.Contains(_processName);

                if (isCursorApp)
                {
                    ShowCursor();
                }
                else
                {
                    await ShowCaret(delayMs);
                }


                CaretContext.LastRect = _rect;
                CaretContext.LastClassName = _className;
                CaretContext.LastMethod = _method;
                CaretContext.LastProcessName = _processName;
                CaretContext.LastHwnd = _hwnd;
                CaretContext.LastUpdated = DateTime.Now;
            }
            finally
            {
                _selectLock.Release();
            }

        }










        public void ShowCursor()
        {
            _cursorHelper.Start();
            _method = CaretMethod.Cursor;
        }



       



        public async Task ShowCaret(int delayMs)
        {
            _cursorHelper.Stop();
            await Task.Delay(delayMs);


            bool contextChanged =
                CaretContext.LastProcessName != _processName ||
                CaretContext.LastClassName != _className || 
                CaretContext.LastMethod == CaretMethod.None;



            if (contextChanged)
            {
                //Console.WriteLine("A1.CTX CHANGE");
                selectCaretMethod();
            }
            else
            {
                //Console.WriteLine("A2.CTX NOT CHANGE");
                useCaretMethod();
            }




            if (_processName == "notepad" && _method == CaretMethod.GUIThreadInfo)
            {
                if (_rect.left == 0 && _rect.top == 0) {
                    UIAHelper.TryGetCaretFromUIA(out _rect);
                }
            }




            _indicatorForm?.BeginInvoke(new Action(() =>
                _indicatorForm.SetPosition(_rect.left, _rect.top, _rect.right - _rect.left, _rect.bottom - _rect.top)
            ));


            if (_rect.left != 0 && _rect.top != 0)
            {
                if (!CommonUtils.IsCaretInEditableArea(_hwnd, _rect, _method))
                {
                    Console.WriteLine("----------!!! CARET HAS POS but NOT VISIBLE");
                    _indicatorForm?.BeginInvoke(new Action(() =>
                        _indicatorForm.HideIndicator()
                     ));
                }
            }



            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{CommonUtils.IsCaretInEditableArea(_hwnd, _rect, _method)}][{_processName}] ({_method}) Caret: L={_rect.left}, T={_rect.top}, R={_rect.right}, B={_rect.bottom}");

            

        }


        public void test()
        {
            Accessibility.IAccessible acc = MSAAHelper.GetAccessibleFromWindow(_hwnd);
            if (acc == null)
                return;

            var text = acc as IAccessibleText;
            if (text == null)
                return;

            text.get_caretOffset(out int caretOffset);
            Console.WriteLine("caretOffset : " + caretOffset);

        }

        public void selectCaretMethod()
        {


        

            if (_processName == "winword" || _processName == "applicationframehost")
             {
                 UIAHelper.TryGetCaretFromUIA(out _rect);
                 _method = CaretMethod.UIA;
             }
             else if (_processName == "notepad")
             {


                GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
                _method = CaretMethod.GUIThreadInfo;

            }
             else if (_processName == "explorer")
             {
                 UIAExplorerHelper.TryGetCaretFromExplorerUIA(out _rect);
                 _method = CaretMethod.ExplorerUIA;
             }
             else
             {

                 if (GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect))
                 {
                     _method = CaretMethod.GUIThreadInfo;
                 }
                 else if (MSAAHelper.TryGetCaretFromMSAA(_hwnd, out _rect))
                 {
                     _method = CaretMethod.MSAA;
                 }
                 else
                 {
                     _rect = new RECT();
                     _method = CaretMethod.None;

                 }
             }




            //GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
            //_method = CaretMethod.GUIThreadInfo;

            //UIAExplorerHelper.TryGetCaretFromExplorerUIA(out _rect);
            //_method = CaretMethod.ExplorerUIA;


            //UIAHelper.TryGetCaretFromUIA(out _rect);
            //_method = CaretMethod.UIA;


        }




        public void useCaretMethod()
        {


            switch (_method)
            {
                case CaretMethod.Selection:
                    OtherHelper.TryGetCaretFromSelectionUIA(out _rect);
                    break;
                case CaretMethod.MSAA:
                    MSAAHelper.TryGetCaretFromMSAA(_hwnd, out _rect);
                    break;
                case CaretMethod.GUIThreadInfo:
                    GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
                    break;
                case CaretMethod.ExplorerUIA:
                    UIAExplorerHelper.TryGetCaretFromExplorerUIA(out _rect);
                    break;
                case CaretMethod.UIA:
                    UIAHelper.TryGetCaretFromUIA(out _rect);
                    break;
                case CaretMethod.MouseClick:
                    MouseHelper.TryGetCaretFromMouseClick(out _rect);
                    break;
                case CaretMethod.Cursor:
                    break;
                case CaretMethod.None:
                    break;
            }


       

        }







    }






}
