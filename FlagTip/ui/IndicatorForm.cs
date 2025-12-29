using FlagTip.Ime;
using FlagTip.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FlagTip.Utils.CommonUtils;


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
        private ImeTracker _imeTracker;


        public IndicatorForm(ImeTracker imeTracker)
        {

            FormBorderStyle = FormBorderStyle.None;
            //BackColor = color;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Width = 6;
            Height = 20;
            Opacity = 0;

            //Opacity = 0.7;

            _imeTracker = imeTracker;

            // 폼 생성 직후 마우스 클릭 투과
            MakeClickThrough();
            SetFlag();
        }

        private void MakeClickThrough()
        {
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        public void SetPosition(int x, int y, int width, int height)
        {
            if (x != 0 && y != 0)
            {
                Location = new Point(x, y);
                Size = new Size(Math.Max(10, 6), Math.Max(6, 18));
                Opacity = 0.7;
            }
            else
            {
                HideIndicator();
            }
        }

        public async Task SetFlag()
        {


            await Task.Delay(10);

            //var imeState = _imeTracker.DetectIme();
            var imeState = _imeTracker.DetectIme();



            if (imeState == ImeState.KOR)
            {
                BackColor = Color.Blue;
            }
            else if(imeState == ImeState.ENG)
            {
                BackColor = Color.Red;
            } else
            {
                BackColor = Color.Yellow;
            }
        }



        public void HideIndicator()
        {


            if (Opacity == 0)
                return;

            Opacity = 0;


        }
    }
}
