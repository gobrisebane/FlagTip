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
        
        private Image _korFlag;
        private Image _jpnFlag;

        private Image _engLowerFlag;
        private Image _engUpperFlag;



        private Image _korFlagSmall;
        private Image _jpnFlagSmall;
        private Image _engLowerFlagSmall;
        private Image _engUpperFlagSmall;

        private Image _korFlagMedium;
        private Image _jpnFlagMedium;        
        private Image _engLowerFlagMedium;
        private Image _engUpperFlagMedium;

        private Image _korFlagLarge;
        private Image _jpnFlagLarge;
        private Image _engLowerFlagLarge;
        private Image _engUpperFlagLarge;


        private const int SMALL_W = 16;
        private const int SMALL_H = 11;

        private const int MEDIUM_W = 21;
        private const int MEDIUM_H = 14;

        private const int LARGE_W = 32;
        private const int LARGE_H = 22;



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


                _korFlagSmall = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_kor_small.png"));
                _jpnFlagSmall = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_jpn_small.png"));
                _engLowerFlagSmall = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_small_lo.png"));
                _engUpperFlagSmall = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_small_up.png"));

                _korFlagMedium = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_kor_medium.png"));
                _jpnFlagMedium = _jpnFlagSmall;
                _engLowerFlagMedium = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_medium_lo.png"));
                _engUpperFlagMedium = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_medium_up.png"));


                _korFlagLarge = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_kor_medium.png"));
                _jpnFlagLarge = _jpnFlagSmall;
                _engLowerFlagLarge = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_medium_lo.png"));
                _engUpperFlagLarge = LoadImageSafe(Path.Combine(basePath, "resources/flag/flag_eng_medium_up.png"));




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



            ApplyFlagSize();


            this.HandleCreated += IndicatorForm_HandleCreated;
            this.FormClosed += IndicatorForm_FormClosed;

        }




        private FlagSizeMode CurrentSizeMode
        {
            get
            {
                return (FlagSizeMode)Properties.Settings.Default.FlagSize;
            }
        }


        private Image GetFlag(ImeState state)
        {
            switch (state)
            {
                case ImeState.KOR:
                    return GetKorFlag(CurrentSizeMode);

                case ImeState.JPN:
                    return GetJpnFlag(CurrentSizeMode);

                case ImeState.ENG_LO:
                    return GetEngLowerFlag(CurrentSizeMode);

                case ImeState.ENG_UP:
                    return GetEngUpperFlag(CurrentSizeMode);

                default:
                    return null;
            }
        }


        private Image GetKorFlag(FlagSizeMode sizeMode)
        {
            switch (sizeMode)
            {
                case FlagSizeMode.Small:
                    return _korFlagSmall;

                case FlagSizeMode.Medium:
                    return _korFlagMedium;

                case FlagSizeMode.Large:
                    return _korFlagLarge;

                default:
                    return _korFlagSmall;
            }
        }

        private Image GetJpnFlag(FlagSizeMode sizeMode)
        {
            switch (sizeMode)
            {
                case FlagSizeMode.Small:
                    return _jpnFlagSmall;

                case FlagSizeMode.Medium:
                    return _jpnFlagMedium;

                case FlagSizeMode.Large:
                    return _jpnFlagLarge;

                default:
                    return _jpnFlagSmall;
            }
        }

        private Image GetEngLowerFlag(FlagSizeMode sizeMode)
        {
            switch (sizeMode)
            {
                case FlagSizeMode.Small:
                    return _engLowerFlagSmall;

                case FlagSizeMode.Medium:
                    return _engLowerFlagMedium;

                case FlagSizeMode.Large:
                    return _engLowerFlagLarge;

                default:
                    return _engLowerFlagSmall;
            }
        }

        private Image GetEngUpperFlag(FlagSizeMode sizeMode)
        {
            switch (sizeMode)
            {
                case FlagSizeMode.Small:
                    return _engUpperFlagSmall;

                case FlagSizeMode.Medium:
                    return _engUpperFlagMedium;

                case FlagSizeMode.Large:
                    return _engUpperFlagLarge;

                default:
                    return _engUpperFlagSmall;
            }
        }





        private Size GetIndicatorSize()
        {
            switch (CurrentSizeMode)
            {
                case FlagSizeMode.Small:
                    return new Size(SMALL_W, SMALL_H);

                case FlagSizeMode.Medium:
                    return new Size(MEDIUM_W, MEDIUM_H);

                case FlagSizeMode.Large:
                    return new Size(LARGE_W, LARGE_H);

                default:
                    return new Size(SMALL_W, SMALL_H);
            }
        }


        private Image GetSmallFlag(ImeState state)
        {
            switch (state)
            {
                case ImeState.KOR:
                    return _korFlagSmall;

                case ImeState.JPN:
                    return _jpnFlagSmall;

                case ImeState.ENG_LO:
                    return _engLowerFlagSmall;

                case ImeState.ENG_UP:
                    return _engUpperFlagSmall;

                default:
                    return null;
            }
        }

        private Image GetMediumFlag(ImeState state)
        {
            switch (state)
            {
                case ImeState.KOR:
                    return _korFlagMedium;

                case ImeState.JPN:
                    return _jpnFlagMedium;

                case ImeState.ENG_LO:
                    return _engLowerFlagMedium;

                case ImeState.ENG_UP:
                    return _engUpperFlagMedium;

                default:
                    return null;
            }
        }



        public void ApplyFlagSize()
        {
            // ✅ 현재 설정 기반으로 사이즈 적용
            Size = GetIndicatorSize();

            // ✅ 이미 표시 중이면 현재 IME 상태로 이미지도 즉시 교체
            if (_hasFlag)
                _flagBox.Image = GetFlag(_curImeState);

            EnsureTopMost();
        }

        public void SetFlagSizeSmall()
        {
            Width = 24;
            Height = 24;
        }

        public void SetFlagSizeMedium()
        {
            Width = 32;
            Height = 32;
        }




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
                Size = GetIndicatorSize();

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
                _curImeState = imeState;

                var img = GetFlag(imeState);
                if (img != null)
                    _flagBox.Image = img;
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



        public Task SetSpecificFlag(ImeState imeState)
        {
            _curImeState = imeState;
            _hasFlag = true;

            var img = GetFlag(imeState);
            if (img != null)
                _flagBox.Image = img;

            return Task.CompletedTask;
        }







        public Task SetCapsLock()
        {
            if (_curImeState != ImeState.ENG_LO &&
                _curImeState != ImeState.ENG_UP)
                return Task.CompletedTask;

            // CapsLock 상태에 따라 표시할 state를 결정
            var stateToShow = IsCapsLockOn() ? ImeState.ENG_UP : ImeState.ENG_LO;

            var img = GetFlag(stateToShow);
            if (img != null)
                _flagBox.Image = img;

            _curImeState = stateToShow;
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
