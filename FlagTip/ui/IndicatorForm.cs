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

        public class IndicatorForm : Form
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


                //Visible = false; 
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
