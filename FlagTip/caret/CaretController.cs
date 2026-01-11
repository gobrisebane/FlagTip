using Accessibility;
using FlagTip.apps;
using FlagTip.Helpers;
using FlagTip.Ime;
using FlagTip.Models;
using FlagTip.Tracking;
using FlagTip.UI;
using FlagTip.Utils;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using UIA;
using UIAutomationClient;
using static FlagTip.Config.AppList;
using static FlagTip.Helpers.MSAAHelper;
using static FlagTip.Input.Tsf.TsfImeStateReader;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;

using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;


namespace FlagTip.Caret
{






    internal class CaretController
    {

        private readonly SemaphoreSlim _selectLock = new SemaphoreSlim(1, 1);



        private IndicatorForm _indicatorForm;
        private ImeTracker _imeTracker;
        private CursorHelper _cursorHelper;
        private CaretMethod _method;
        private IntPtr _hwnd;
        private string _processName;
        private string _className;
        private RECT _rect;
        uint _pid;
        private bool _contextChanged = true;


        public CaretController(IndicatorForm indicatorForm, ImeTracker imeTracker)
        {
            _indicatorForm = indicatorForm;
            _imeTracker =  imeTracker;
            _cursorHelper = new CursorHelper(_indicatorForm);


            _indicatorForm.ForegroundHandled += OnForegroundHandled;
        }




        public async Task OnForegroundAppCreatedAsync(IntPtr hwnd)
        {
            await Task.Delay(300);
            await SelectMode();
        }


        private void OnForegroundHandled()
        {
            SelectMode();
        }


        private CaretTracker _tracker;



        public void AttachTracker(CaretTracker tracker)
        {
            _tracker = tracker;
        }



        private CancellationTokenSource _typingCts;
        private readonly TimeSpan TypingResumeDelay = TimeSpan.FromSeconds(5);



        


        private DateTime _typingHoldUntilUtc = DateTime.MinValue;
        private const int TYPING_HOLD_MS = 2000;



        public async void NotifyTyping()
        {

            _typingHoldUntilUtc = DateTime.UtcNow.AddMilliseconds(TYPING_HOLD_MS);



            _tracker?.Pause();

                   // 기존 타이머 취소
            _typingCts?.Cancel();
            _typingCts?.Dispose();

            _typingCts = new CancellationTokenSource();
            var token = _typingCts.Token;


            if (!_selectAllLocked)
            {
                _selectAllLocked = true;
                await SelectMode();
            }


            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TypingResumeDelay, token);

                    if (!token.IsCancellationRequested)
                    {
                        _tracker?.Resume();
                    }

                    

                }
                catch (OperationCanceledException) { }
            });
        }


  
        public void NotifyCaretMove()
        {
            _tracker?.Resume();
        }


        private bool _selectAllLocked = true;

        public void NotifySelectAll()
        {
            _selectAllLocked = false;
        }

    



        public async Task NotifyImeToggle()
        {
           
            SetFlag();
            await SelectMode();

        }

        public async Task NotifyCapsLockToggle()
        {
            SetFlag();            
        }








        public async Task OnKeyChangedAsync(int delayMs = 50)
        {
            await SelectMode(delayMs);
        }




        public async Task SelectDoubleClick()
        {
            await SelectMode(10);

        }


        private void UpdateForegroundContext()
        {
            _hwnd = GetForegroundWindow();
            _processName = GetProcessName(_hwnd);

            StringBuilder classNameBuilder = new StringBuilder(256);
            GetClassName(_hwnd, classNameBuilder, classNameBuilder.Capacity);
            _className = classNameBuilder.ToString();

            GetWindowThreadProcessId(_hwnd, out _pid);
        }

        private bool IsContextChanged()
        {
            return
                CaretContext.LastProcessName != _processName ||
                CaretContext.LastClassName != _className ||
                CaretContext.LastMethod == CaretMethod.None;
        }



  


        public async Task MultiSelectMode(int count = 3)
        {

         
            

            await Task.Delay(50);
            await SelectMode();

            for (int i = 0; i < count; i++)
            {
                if (IsProcessCursorApp())
                    break;

                await Task.Delay(80);
                await SelectMode();
            }


        }

        public async Task MultiSelectModeBrowser(int count = 3)
        {

            if( IsProcessBrowserApp() )
            {

                await SelectMode();

                for (int i = 0; i < count; i++)
                {
                    if (IsProcessCursorApp())
                        break;

                    await Task.Delay(100);
                    await SelectMode();
                }
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

                UpdateForegroundContext();
                _method = CaretContext.LastMethod;
                _rect = CaretContext.LastRect;

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


        public async Task ShowCaret(int delayMs)
        {
            _cursorHelper.Stop();
            await Task.Delay(delayMs);




            bool contextChanged = IsContextChanged();

            if (contextChanged)
            {
                //Console.WriteLine("A1.CTX CHANGE");
                selectCaretMethod();
                checkSpecificApps();





            }
            else
            {
                //Console.WriteLine("A2.CTX NOT CHANGE");
                useCaretMethod();
            }





            // 메모장 더블클릭 때문에 추가
            if (_processName == "notepad" && _method == CaretMethod.GUIThreadInfo)
            {
                if (_rect.left == 0 && _rect.top == 0)
                {
                    UIAHelper.TryGetCaretFromUIA(out _rect);
                }
            }



            SetFlagPosition();




            if (_rect.left != 0 && _rect.top != 0)
            {
                if (!CommonUtils.IsCaretInEditableArea(_hwnd, _rect, _method))
                {
                    //Console.WriteLine("----------!!! CARET HAS POS but NOT VISIBLE");
                    _indicatorForm?.BeginInvoke(new Action(() =>
                        _indicatorForm.HideIndicator()
                     ));
                }
            }




            //Console.WriteLine("_hwnd : " + _hwnd);
            //Console.WriteLine($" >>>>> DPI = {NativeMethods.GetDpiForWindow(_hwnd)}");

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{CommonUtils.IsCaretInEditableArea(_hwnd, _rect, _method)}][{_processName}][{_pid}] ({_method}) Caret: L={_rect.left}, T={_rect.top}, W={_rect.right - _rect.left}, B={_rect.bottom}");


        }





        public async Task checkSpecificApps()
        {
            // VS 디버그 모드때문에 반영

            if (_processName == "devenv")
            {
                await Task.Delay(50);
                SetFlag();
            }

        }




        public void ShowCursor()
        {
            _cursorHelper.Start();
            _method = CaretMethod.Cursor;
        }








       




        public async Task OnKeyTest()
        {
            IntPtr hIMC = ImmGetContext(_hwnd);
            if (hIMC == IntPtr.Zero)
                //return false;

            ImmGetConversionStatus(hIMC, out int conv, out _);
            ImmReleaseContext(_hwnd, hIMC);




            //Console.WriteLine("(conv & ImeNative.IME_CMODE_NATIVE) != 0 : " + (conv&ImeNative.IME_CMODE_NATIVE));
            //Console.WriteLine("conv : " + conv);
            //Console.WriteLine("IME_CMODE_NATIVE : " + IME_CMODE_NATIVE);
        }









        public void SetFlagPosition()
        {
            _indicatorForm?.BeginInvoke(new Action(() =>
            {
                _indicatorForm.SetPosition(_rect.left, _rect.top, _rect.right - _rect.left, 
                    _rect.bottom - _rect.top, _processName);
            }
           ));
        }



        public void SetFlag()
        {
            _indicatorForm?.BeginInvoke(new Action(() =>
            {
                _indicatorForm.SetFlag();
            }
            ));
        }



        public void SetCapsLock()
        {
            _indicatorForm?.BeginInvoke(new Action(() =>
            {
                _indicatorForm.SetCapsLock();

            }
            ));
        }



        public void selectCaretMethod()
        {

            if (_processName == "winword"
                ||  _processName == "applicationframehost" 
                || _processName == "devenv"
                )
             {
                 UIAHelper.TryGetCaretFromUIA(out _rect);
                 _method = CaretMethod.UIA;
            }
             else if ( IsProcessBrowserApp() )
            {
                MSAAHelper.TryGetCaretFromMSAA(_hwnd, out _rect);
                _method = CaretMethod.MSAA;
            }
            else if (_processName == "notepad")
            {
                GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
                _method = CaretMethod.GUIThreadInfo;
            }
             else if (_processName == "explorer")
             {
                UIAExplorerHelper.TryGetCaretFromUIAExplorer(_hwnd, out _rect);
                _method = CaretMethod.UIAExplorer;
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
                else if (UIAHelper.TryGetCaretFromUIA(out _rect))
                {
                    _method = CaretMethod.UIA;
                }
                else
                {
                    _rect = new RECT();
                    _method = CaretMethod.None;

                }
             }




            //GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
            //_method = CaretMethod.GUIThreadInfo;


            //UIAExplorerHelper.TryGetCaretFromUIAExplorer(out _rect);
            //_method = CaretMethod.UIAExplorer;


            //MSAAHelper.TryGetCaretFromMSAA(_hwnd, out _rect);
            //_method = CaretMethod.MSAA;


            //UIAExplorerHelper.TryGetCaretFromUIAExplorer(out _rect);
            //_method = CaretMethod.UIAExplorer;


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
                case CaretMethod.UIAorGUI:
                    UIAorGUIHelper.TryGetCaretFromUIAorGUI(_hwnd, out _rect);
                    break;
                case CaretMethod.MSAA:
                    MSAAHelper.TryGetCaretFromMSAA(_hwnd, out _rect);
                    break;
                case CaretMethod.GUIThreadInfo:
                    GUIThreadHelper.TryGetCaretFromGUIThreadInfo(_hwnd, out _rect);
                    break;
                case CaretMethod.UIAExplorer:
                    UIAExplorerHelper.TryGetCaretFromUIAExplorer(_hwnd, out _rect);
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
