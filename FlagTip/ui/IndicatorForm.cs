using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public IndicatorForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Red;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Width = 6;
            Height = 20;
            Opacity = 0;

            //Opacity = 0.7;


            // 폼 생성 직후 마우스 클릭 투과
            MakeClickThrough();
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

        public void HideIndicator()
        {

            if (Opacity == 0)
                return;

            Opacity = 0;


        }
    }
}
