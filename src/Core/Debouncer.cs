using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vtex.Toolbelt.Core
{
    public class Debouncer
    {
        private readonly TimeSpan delay;
        private CancellationTokenSource cancellationTokenSource;

        public Debouncer(TimeSpan delay)
        {
            this.delay = delay;
        }

        public void Debounce(Action action)
        {
            if (this.cancellationTokenSource != null)
                this.cancellationTokenSource.Cancel();

            this.cancellationTokenSource = new CancellationTokenSource();
            Task.Delay(delay, this.cancellationTokenSource.Token)
                .ContinueWith((task) => action(), TaskContinuationOptions.NotOnCanceled);
        }
    }
}