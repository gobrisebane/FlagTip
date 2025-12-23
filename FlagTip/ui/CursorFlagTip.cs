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

        public CursorFlagTip()
        {
            _indicatorForm = new IndicatorForm();
            _indicatorForm.Hide(); // 처음에는 안보이게

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

        // 켜기
        public void Start()
        {
            if (!IsRunning)
            {
                _indicatorForm.Show();
                _timer.Start();
                IsRunning = true;
            }
        }

        // 끄기
        public void Stop()
        {
            if (IsRunning)
            {
                _timer.Stop();
                _indicatorForm.Hide();
                IsRunning = false;
            }
        }
    }
}