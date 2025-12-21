using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.UI
{

        internal class IndicatorForm : Form
        {


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
                //Visible = false; // <- 처음에는 화면에 안 나타나도록 설정

                //this.Paint += (s, e) =>
                //{
                //    using (var pen = new Pen(Color.Blue, 2)) // 테두리 색
                //    {
                //        e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                //    }
                //};

                //this.Visible = false;

            }


            public void SetPosition(int x, int y, int width, int height)
            {

                //if (x != 0 && y != 0 && width != 0 && height != 0)
                if (x != 0 && y != 0)

                {
                    Location = new Point(x, y);
                    //Size = new Size(Math.Max(width, 6), Math.Max(height, 18));
                    //Size = new Size(Math.Max(10, 6), Math.Max(height, 18));
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
