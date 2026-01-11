using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.Tray
{
    internal class TrayController : IDisposable
    {
        private NotifyIcon _trayIcon;

        public event Action OptionRequested;
        public event Action AboutRequested;  

        public TrayController()
        {
            InitializeTrayIcon();
        }

       
        private void InitializeTrayIcon()
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("옵션", null, (_, __) =>
            {
                OptionRequested?.Invoke();
            });

            menu.Items.Add("About", null, (_, __) =>
            {
                AboutRequested?.Invoke();
            });

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add("종료", null, (_, __) =>
            {
                Application.Exit();
            });

            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = menu,
                Text = "FlagTip",
                Visible = true
            };

            _trayIcon.DoubleClick += (s, e) =>
            {
                OptionRequested?.Invoke();
            };
        }

        public void Dispose()
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
    }
}