using FlagTip.Ime;
using FlagTip.models;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.UI
{
    public class IndicatorForm : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;

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
            _imeTracker = imeTracker;

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Width = 6;
            Height = 20;
            Opacity = 0;

            MakeClickThrough();
            _ = SetFlag();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            EnsureTopMost();
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

        public void SetPosition(int x, int y, int width, int height)
        {
            if (x != 0 && y != 0)
            {
                Location = new Point(x, y);
                Size = new Size(Math.Max(10, 6), Math.Max(6, 18));
                Opacity = 0.7;

                EnsureTopMost();
            }
            else
            {
                HideIndicator();
            }
        }

        public async Task SetFlag()
        {
            await Task.Delay(10);

            ImeState imeState = _imeTracker.DetectIme();

            if (imeState == ImeState.KOR)
                BackColor = Color.Blue;
            else if (imeState == ImeState.ENG)
                BackColor = Color.Red;
            else
                BackColor = Color.Yellow;
        }

        public void HideIndicator()
        {
            if (Opacity == 0)
                return;

            Opacity = 0;
        }
    }
}
