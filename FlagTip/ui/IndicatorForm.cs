using FlagTip.Ime;
using FlagTip.models;
using FlagTip.watchers;
using System;
using System.Collections.Generic;
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

        private Dictionary<uint, ImeState> _imeStateMap;

        public IndicatorForm(ImeTracker imeTracker)
        {

            _imeStateMap = new Dictionary<uint, ImeState>();

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



        private async Task HandleForegroundAsync(IntPtr hwnd, uint pid, string processName)
        {

            _hasFlag = false;

            //HideIndicator();
            //await Task.Delay(1000);




           /* Console.WriteLine("--------- processName : " + processName);
            Console.WriteLine("--------- pid : " + pid);


            if (_imeStateMap.TryGetValue(pid, out ImeState savedState))
            {
                // 👉 이전에 쓰던 IME 상태 복원
                //_imeTracker.SetIme(savedState); // 네가 이미 가진 메서드라고 가정

                SetSpecificFlag(savedState);
                Console.WriteLine($"[111. IME RESTORE] pid={pid}, state={savedState}");
            }
            else
            {
                // 👉 처음 보는 pid → 현재 IME 상태 감지해서 저장
                await Task.Delay(30); // IME 안정화용
                ImeState currentState = _imeTracker.DetectIme();
                _imeStateMap[pid] = currentState;

                Console.WriteLine($"[222. IME SAVE] pid={pid}, state={currentState}");
            }*/




            Console.WriteLine(
                string.Join(", ",
                    _imeStateMap.Select(kv => $"{kv.Key}:{kv.Value}")
                )
            );



            //ShowIndicator();





            /*await Task.Delay(50);
            await SetFlag();

            //ShowIndicator();

            for (int i = 0; i < 3; i++)
            {
                await SetFlag();
                await Task.Delay(50);
            }*/




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
            int height)
        {

            if (x != 0 && y != 0 && width > 0)
            {

                Location = new Point(x + OFFSET_X, y + OFFSET_Y);
                Size = new Size(INDICATOR_WIDTH, INDICATOR_HEIGHT);


                if (!_hasFlag)
                {
                    _hasFlag = true;
                    await SetFlag();
                }


                ShowIndicator();


            }
            else
            {

                //Console.WriteLine("22222. FLAG WORKS");

                HideIndicator();
            }

            
        }


 




        public async Task SetFlag()
        {


            //HideIndicator();
            //await Task.Delay(1000);



            await Task.Delay(50);
            ImeState imeState = _imeTracker.DetectIme();

            Console.WriteLine("....imeState : " + imeState);

            switch (imeState)
            {
                case ImeState.KOR:
                    Console.WriteLine("h111");
                    _flagBox.Image = _korFlag;
                    _curImeState = ImeState.KOR;
                    break;

                case ImeState.ENG_LO:
                    Console.WriteLine("h222");
                    _flagBox.Image = _engLowerFlag;
                    _curImeState = ImeState.ENG_LO;
                    break;

                case ImeState.ENG_UP:
                    Console.WriteLine("h333");
                    _flagBox.Image = _engUpperFlag;
                    _curImeState = ImeState.ENG_UP;
                    break;

                default:
                    break;
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
            //EnsureTopMost();
        }

        public void HideIndicator()
        {
            Hide();
        }
    }
}
