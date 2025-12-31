using FlagTip.Caret;
using FlagTip.Hooking;
using FlagTip.Ime;
using FlagTip.Tracking;
using FlagTip.UI;
using FlagTip.watchers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FlagTip.Hooking.KeyboardHook;
using static FlagTip.Hooking.MouseHook;
using static FlagTip.Utils.NativeMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FlagTip
{
    public partial class MainForm : Form
    {

        private IndicatorForm _indicatorForm;
        private CaretController _caretController;
        private CaretTracker _tracker;
        //private ForegroundWatcher _foregroundWatcher;

        private IntPtr _mouseHook;
        private LowLevelMouseProc _mouseProc;

        private IntPtr _keyboardHook;
        private LowLevelKeyboardProc _keyboardProc;

        private ImeTracker _imeTracker;


        public MainForm()
        {
            InitializeComponent();
            
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;


            Opacity = 0;

            Init();
        }



        private void Init()
        {

            _imeTracker = new ImeTracker();

            _indicatorForm = new IndicatorForm(_imeTracker);
            _indicatorForm.Show();

            _caretController = new CaretController(_indicatorForm, _imeTracker);



            _mouseProc = (nCode, wParam, lParam) =>
                MouseHookCallback(nCode, wParam, lParam, _mouseHook, _caretController);

            _mouseHook = SetMouseHook(_mouseProc);



            _keyboardProc = (nCode, wParam, lParam) =>
                    KeyboardHook.KeyboardHookCallback(
                    nCode,
                    wParam,
                    lParam,
                    _keyboardHook,
                    _caretController);

            _keyboardHook = KeyboardHook.SetKeyboardHook(_keyboardProc);



            _tracker = new CaretTracker(_caretController);
            _caretController.AttachTracker(_tracker);
            _tracker.Start();




            //_foregroundWatcher = new ForegroundWatcher();
            //_foregroundWatcher.ForegroundChanged += OnForegroundChanged;
            //_foregroundWatcher.Start();




        }



       /* private async void OnForegroundChanged(IntPtr hwnd, string processName)
        {

            //await Task.Delay(50);
            //await Task.Delay(100);
             //_indicatorForm.HideIndicator();

            BeginInvoke(new Action(async () =>
            {
                Console.WriteLine($"--------------Foreground: {processName}");

                //await Task.Delay(50);
                //await Task.Delay(100);
                //_indicatorForm.HideIndicator();
                //await Task.Delay(1000);

                await _indicatorForm.SetFlag();
                await _caretController.SelectMode();


                for (int i = 0; i < 3; i++)
                {
                    await _indicatorForm.SetFlag();
                    await Task.Delay(50);
                }


            }));
        }*/

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            _tracker?.Dispose();
            //_foregroundWatcher?.Dispose();
            UnhookWindowsHookEx(_mouseHook);

            base.OnFormClosing(e);
        }

        
    }
}
