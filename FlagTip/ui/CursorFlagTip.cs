using FlagTip.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlagTip.ui
{
    public class CursorFlagTip
    {
        private Timer _timer;
        private IndicatorForm _indicatorForm;
        private int offsetX = 10;
        private int offsetY = 10;

        public bool IsRunning { get; private set; } = false;

        public CursorFlagTip(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;

            //_indicatorForm.Hide(); // 처음에는 안보이게

            _timer = new Timer();
            _timer.Interval = 32; // 약 30fps
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point cursorPos = Cursor.Position;
            int x = cursorPos.X + offsetX;
            int y = cursorPos.Y + offsetY;
            int width = 10;
            int height = 18;

            _indicatorForm.SetPosition(x, y, width, height);
        }

        public void Start()
        {
            Console.WriteLine("......IsRunning : " + IsRunning);
            if (!IsRunning)
            {
                Console.WriteLine("....................show works1");

                _indicatorForm.Show();
                Console.WriteLine("....................show works2");

                _timer.Start();
                Console.WriteLine("....................show works3");

                IsRunning = true;
                Console.WriteLine("....................show works4");


            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _timer.Stop();
                //_indicatorForm.Hide();
                IsRunning = false;
            }
        }
    }
}