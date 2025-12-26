using FlagTip.Caret;
using FlagTip.Hooking;
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
        private ForegroundWatcher _foregroundWatcher;

        private IntPtr _mouseHook;
        private LowLevelMouseProc _mouseProc;

        private IntPtr _keyboardHook;
        private LowLevelKeyboardProc _keyboardProc;





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
            _indicatorForm = new IndicatorForm(Color.Red);
            _indicatorForm.Show();



            _caretController = new CaretController(_indicatorForm);



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



            //_tracker = new CaretTracker(_caretController);
            //_tracker.Start();



            //_foregroundWatcher = new ForegroundWatcher();
            //_foregroundWatcher.ForegroundChanged += OnForegroundChanged;
            //_foregroundWatcher.Start();









            /*Task.Run(() =>
            {
                string[] apps = { "EXCEL", "devenv", "notepad", "explorer", "WhatsApp.Root" };
                while (true)
                {
                    foreach (var app in apps)
                    {
                        var proc = GetProcessByName(app);
                        if (proc != null)
                        {
                            Console.WriteLine($"Activating {app}");
                            SetForegroundWindow(proc.MainWindowHandle);
                        }
                        else
                        {
                            Console.WriteLine($"Process {app} not found");
                        }

                        Thread.Sleep(1000); // 2초 대기
                    }
                }
            });*/


        }

        static Process GetProcessByName(string name)
        {
            var procs = Process.GetProcessesByName(name);
            return procs.Length > 0 ? procs[0] : null;
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);



        private async void OnForegroundChanged(IntPtr hwnd, string processName)
        {
            await Task.Delay(50); 
            BeginInvoke(new Action(() =>
            {
                Console.WriteLine($"--------------Foreground: {processName}");
                _caretController.SelectMode();
            }));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            _tracker?.Dispose();
            _foregroundWatcher?.Dispose();
            UnhookWindowsHookEx(_mouseHook);

            base.OnFormClosing(e);
        }

        
    }
}
