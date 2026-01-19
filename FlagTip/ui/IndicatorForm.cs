using FlagTip.Caret;
using FlagTip.Ime;
using FlagTip.Models;
using FlagTip.Watchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
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
        private Image _jpnFlag;
        private Image _engLowerFlag;
        private Image _engUpperFlag;

        private ImeState _curImeState;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;


        public int OFFSET_X;
        public int OFFSET_Y;

        private const int INDICATOR_WIDTH = 16;
        private const int INDICATOR_HEIGHT = 11;


        // 시작페이지에 맞을만한 사이즈
        //private const int INDICATOR_WIDTH = 45;
        //private const int INDICATOR_HEIGHT = 23;

        //private const int INDICATOR_WIDTH = 35;
        //private const int INDICATOR_HEIGHT = 18;



        private readonly ImeTracker _imeTracker;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |=
                    WS_EX_TOOLWINDOW |
                    WS_EX_NOACTIVATE |
                    WS_EX_LAYERED |
                    WS_EX_TRANSPARENT; 
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


            Width = 16;
            Height = 11;


            BackColor = Color.FromArgb(245, 245, 245);
            TransparencyKey = Color.FromArgb(245, 245, 245);

            OFFSET_X = Properties.Settings.Default.OffsetX;
            OFFSET_Y = Properties.Settings.Default.OffsetY;

            double savedOpacity = Properties.Settings.Default.Opacity;
            Opacity = savedOpacity;







            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                Log($"BasePath = {basePath}");

                _korFlag = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_kor.png"));
                _jpnFlag = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_jpn.png"));
                _engLowerFlag = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_lo.png"));
                _engUpperFlag = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_up.png"));

                //_korFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_kor.png"));
                //_engLowerFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_eng_lo.png"));
                //_engUpperFlag = Image.FromFile(Path.Combine(basePath, "resources/flag/flag_eng_up.png"));

            }
            catch (Exception ex)
            {
                Log($"[IMAGE LOAD ERROR] {ex}");
            }


       
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

/*
        public IndicatorForm2(ImeTracker imeTracker)
        {
            _foregroundWatcher = new ForegroundWatcher();
            _imeTracker = imeTracker;

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;

            Width = 16;
            Height = 11;

            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;

            OFFSET_X = Properties.Settings.Default.OffsetX;
            OFFSET_Y = Properties.Settings.Default.OffsetY;
            Opacity = Properties.Settings.Default.Opacity;

            var redBox = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Red
            };

            Controls.Add(redBox);

            HandleCreated += IndicatorForm_HandleCreated;
            FormClosed += IndicatorForm_FormClosed;
        }
*/
        public void SetOpacity(double opacity)
        {
            this.Opacity = opacity;

            // Settings에 즉시 저장
            Properties.Settings.Default.Opacity = opacity;
            Properties.Settings.Default.Save();
        }

        public double GetOpacity()
        {
            // 현재 Settings에 저장된 값 반환
            return Properties.Settings.Default.Opacity;
        }

        private async void IndicatorForm_HandleCreated(object sender, EventArgs e)
        {

            
            try
            {
                await SetFlag();
            }
            catch (Exception ex)
            {
                Log($"[HandleCreated SetFlag ERROR] {ex}");
            }
            

            _foregroundWatcher.ForegroundChanged += OnForegroundChanged;
            _foregroundWatcher.Start();
        }
        private void IndicatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _foregroundWatcher?.Dispose();
        }



   
        private void OnForegroundChanged(IntPtr hwnd, uint pid, string processName)
        {

            if (!IsHandleCreated)
                return;
            
            BeginInvoke(new Action(() =>
            {
                _ = HandleForegroundAsync(hwnd, pid,  processName);
            }));




        }


        private bool _hasFlag = false;
        public event Action ForegroundHandled;


        private async Task HandleForegroundAsync(IntPtr hwnd, uint pid, string processName)
        {


            _hasFlag = false;

            await Task.Delay(50);
            await SetFlag();
            ForegroundHandled?.Invoke();


            for (int i = 0; i < 6; i++)
            {
                await Task.Delay(150);
                await SetFlag();
            }



        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            EnsureTopMost();
        }

  

  
        public async Task SetPosition(int x, int y, int width, 
            int height, String processName = null)
        {

            if (x != 0 && y != 0 && width > 0)
            {

                Location = new Point(x + OFFSET_X, y + OFFSET_Y);
                Size = new Size(INDICATOR_WIDTH, INDICATOR_HEIGHT);

                if (!_hasFlag)
                {
                            // 크롬-파일업로드 대비
                    _hasFlag = true;
                    await SetFlag();
                }
                ShowIndicator();
            }
            else
            {

                HideIndicator();


          
            }


        }


 




        public async Task SetFlag()
        {

            try
            {

                await Task.Delay(50);
                ImeState imeState = _imeTracker.DetectIme();

                _hasFlag = true;




                switch (imeState)
                {
                    case ImeState.KOR:
                        _flagBox.Image = _korFlag;
                        _curImeState = ImeState.KOR;
                        break;

                    case ImeState.JPN:
                        _flagBox.Image = _jpnFlag;
                        _curImeState = ImeState.JPN;
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

            }
            catch (Exception ex)
            {
                Log($"[SetFlag ERROR] {ex}");
            }


        }
        private Image LoadImageSafe(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var img = Image.FromStream(fs))
            {
                // 스트림에서 완전히 분리
                return new Bitmap(img);
            }
        }



        public async Task SetSpecificFlag(ImeState imeState)
        {
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
            EnsureTopMost();
        }

        public void HideIndicator()
        {
            Hide();
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



        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }

            base.WndProc(ref m);
        }

    }
}
