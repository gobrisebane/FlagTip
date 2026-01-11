using FlagTip.Caret;
using FlagTip.Models;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.Helpers
{
    public class CursorHelper
    {

        private System.Threading.Timer _timer;
        private IndicatorForm _indicatorForm;
        private int offsetX = 10;
        private int offsetY = 10;

        public bool IsRunning { get; private set; } = false;

        public CursorHelper(IndicatorForm indicatorForm)
        {
            _indicatorForm = indicatorForm;
            _timer = new System.Threading.Timer(_ =>
            {
                UpdateCursor();
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        
        private void UpdateCursor()
        {
            Point cursorPos = Cursor.Position;
            int x = cursorPos.X + offsetX;
            int y = cursorPos.Y + offsetY;
            int width = 10;
            int height = 18;

            // UI 스레드 보장
            _indicatorForm.BeginInvoke(new Action(() =>
            {
                _indicatorForm.SetPosition(x, y, width, height);
            }));
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _indicatorForm.BeginInvoke(new Action(() =>
                {
                    _indicatorForm.Show();
                }));

                IsRunning = true;
                _timer.Change(0, 31); // 즉시 시작, 30fps

            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                IsRunning = false;
            }
        }
    }
}
