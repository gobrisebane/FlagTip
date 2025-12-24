using FlagTip.caret;
using FlagTip.Helpers;
using FlagTip.ui;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Tracking
{
    internal class CaretTracker
    {

        private Caret _caret;
        private Thread _thread;

        public CaretTracker(Caret caret )
        {
            _caret= caret;
            _thread = new Thread(Run) { IsBackground = true };
        }

        public void Start() => _thread.Start();


        private void Run()
        {

            while (true)
            {
                try
                {
                    _caret.show().GetAwaiter().GetResult();

                }
                catch { }
                Thread.Sleep(800);
            }
        }


    }
}
