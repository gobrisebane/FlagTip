using FlagTip.Caret;
using FlagTip.Helpers;
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
    public class CaretTracker : IDisposable
    {
        private readonly CaretController _caretController;
        private CancellationTokenSource _cts;
        private Task _task;
        private volatile bool _paused;


        public CaretTracker(CaretController caretController)
        {
            _caretController = caretController;
            _cts = new CancellationTokenSource();
        }

        public void Start()
        {
            if (_task != null && !_task.IsCompleted)
                return;

            _task = Task.Run(RunAsync);
        }


        public void Pause()
        {
            _paused = true;
        }

        public void Resume()
        {
            _paused = false;
        }

        private async Task RunAsync()
        {
            var token = _cts.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_paused)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }

                    await _caretController.SelectMode();
                    await Task.Delay(500, token);

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // log optional
                }
            }
        }


        public void Stop()
        {
            if (_cts.IsCancellationRequested)
                return;

            _cts.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }
    }
}