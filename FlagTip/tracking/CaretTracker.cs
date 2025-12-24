using FlagTip.Caret;
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
    internal class CaretTracker : IDisposable
    {
        private readonly CaretController _caretController;
        private CancellationTokenSource _cts;
        private Task _task;

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

        private async Task RunAsync()
        {
            var token = _cts.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await _caretController.Show();   // async 유지
                    await Task.Delay(2000, token); // ⭐ 핵심
                }
                catch (OperationCanceledException)
                {
                    break; // Stop 즉시 반응
                }
                catch
                {
                    // 필요 시 로그
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