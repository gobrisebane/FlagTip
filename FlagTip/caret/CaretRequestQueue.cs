using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlagTip.caret
{
    internal static class CaretRequestQueue
    {
        static CancellationTokenSource _cts;

        internal static void Request()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _ = HandleAsync(_cts.Token);
        }

        static async Task HandleAsync(CancellationToken token)
        {
            try
            {
                int delay = CaretContext.IsWebView2 ? 60 : 30;

                await Task.Delay(delay, token);
                if (token.IsCancellationRequested)
                    return;

                //await caret.show();
            }
            catch (TaskCanceledException) { }
        }
    }
}
