using FlagTip.Ime;
using FlagTip.models;
using FlagTip.watchers;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.UI
{
    public class IndicatorForm : Form
    {


        private ForegroundWatcher _foregroundWatcher;

        private PictureBox _flagBox;
        private Image _korFlag;
        private Image _engLowerFlag;
        private Image _engUpperFlag;

        private ImeState _curImeState;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;


        private const int OFFSET_X = 3;
        private const int OFFSET_Y = 20;
        //private const int INDICATOR_WIDTH = 15;
        //private const int INDICATOR_HEIGHT = 10;

        private const int INDICATOR_WIDTH = 16;
        private const int INDICATOR_HEIGHT = 11;
        private const double FLAG_OPACITY = 0.70;


        private readonly ImeTracker _imeTracker;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        public IndicatorForm(ImeTracker imeTracker)
        {

            _foregroundWatcher = new ForegroundWatcher();

            _imeTracker = imeTracker;

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Width = 6;
            Height = 20;
            Opacity = FLAG_OPACITY;


            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            _korFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_kor_2.png"));
            //_engLowerFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_eng_lo.png"));
            _engLowerFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_eng_up_2.png"));
            _engUpperFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_eng_up_2.png"));

            _flagBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };

            Controls.Add(_flagBox);


            this.HandleCreated += IndicatorForm_HandleCreated;
            this.FormClosed += IndicatorForm_FormClosed;

        }

        private void IndicatorForm_HandleCreated(object sender, EventArgs e)
        {
            MakeClickThrough();
            _ = SetFlag();

            _foregroundWatcher.ForegroundChanged += OnForegroundChanged;
            _foregroundWatcher.Start();
        }
        private void IndicatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _foregroundWatcher?.Dispose();
        }



        private void OnForegroundChanged(IntPtr hwnd, string processName)
        {

            if (!IsHandleCreated)
                return;
            
            BeginInvoke(new Action(() =>
            {
                _ = HandleForegroundAsync(hwnd, processName);
            }));
        }




        private async Task HandleForegroundAsync(IntPtr hwnd, string processName)
        {

            //HideIndicator();
            //await Task.Delay(1000);

            Console.WriteLine("--------- processName : " + processName);

            await Task.Delay(50);
            await SetFlag();

            //ShowIndicator();

            for (int i = 0; i < 3; i++)
            {
                await SetFlag();
                await Task.Delay(50);
            }



        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //EnsureTopMost();
        }

        private void EnsureTopMost()
        {
            if (!IsHandleCreated)
                return;

            SetWindowPos(
                Handle,
                HWND_TOPMOST,
                0,
                0,
                0,
                0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private void MakeClickThrough()
        {
            int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
            SetWindowLong(Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }


  
        public async Task SetPosition(int x, int y, int width, 
            int height, bool contextChange = false)
        {

            if (x != 0 && y != 0 && width > 0)
            {

                Location = new Point(x + OFFSET_X, y + OFFSET_Y);
                Size = new Size(INDICATOR_WIDTH, INDICATOR_HEIGHT);


                if (contextChange)
                {

                    HideIndicator();
                    Console.WriteLine("1. context change.. need delay");
                    await Task.Delay(50);

                    ShowIndicator();
                    //dont imeCheck
                }
                else if (!contextChange)
                {
                    Console.WriteLine("2. context not change.. instant");
                    ShowIndicator();
                }


            }
            else
            {
                HideIndicator();
            }

            
        }



        public async Task SetFlag()
        {


            //HideIndicator();

            await Task.Delay(50);
            ImeState imeState = _imeTracker.DetectIme();

            switch (imeState)
            {
                case ImeState.KOR:
                    _flagBox.Image = _korFlag;
                    _curImeState = ImeState.KOR;
                    break;

                case ImeState.ENG_LO:
                    _flagBox.Image = _engLowerFlag;
                    _curImeState = ImeState.ENG_LO;
                    break;

                case ImeState.ENG_UP:
                    _flagBox.Image = _engUpperFlag;
                    _curImeState = ImeState.ENG_UP;
                    break;

                default:
                    break;
            }
            
            //await Task.Delay(1000);
            //ShowIndicator();
        }



        public Task SetCapsLock()
        {

            if (_curImeState != ImeState.ENG_LO &&
                _curImeState != ImeState.ENG_UP)
                return Task.CompletedTask;

            if ( IsCapsLockOn() )
            {
                _flagBox.Image = _engLowerFlag;
            }
            else // ENG_UP
            {
                _flagBox.Image = _engUpperFlag;
            }
            return Task.CompletedTask;
        }

        public void ShowIndicator()
        {
            Show();
            //EnsureTopMost();
        }

        public void HideIndicator()
        {
            Hide();
        }
    }
}
