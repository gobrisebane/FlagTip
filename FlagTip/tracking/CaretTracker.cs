using FlagTip.Helpers;
using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;
using FlagTip.caret;


namespace FlagTip.Tracking
{
    internal class CaretTracker
    {

        private Caret _caret;
        private IndicatorForm _indicatorForm;
        private Thread _thread;

        public CaretTracker(IndicatorForm indicatorForm, Caret caret)
        {
            _caret= caret;
            _indicatorForm = indicatorForm;
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
                Thread.Sleep(1000);
            }
        }


    }
}
