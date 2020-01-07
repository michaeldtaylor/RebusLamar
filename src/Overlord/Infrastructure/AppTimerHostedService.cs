using System;
using System.Threading;
using System.Threading.Tasks;

namespace Overlord.Infrastructure
{
    public abstract class AppTimerHostedService : IAppTimerHostedService, IDisposable
    {
        private readonly IAppNameProvider _appNameProvider;

        private Timer _timer;

        public virtual TimeSpan Period { get; set; } = TimeSpan.FromSeconds(30);

        protected AppTimerHostedService(IAppNameProvider appNameProvider)
        {
            _appNameProvider = appNameProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var appTimerState = new AppTimerState
            {
                AppName = _appNameProvider.GetAppName()
            };

            _timer = new Timer(async s => await OnTimerElapsedAsync((AppTimerState)s, cancellationToken), appTimerState, TimeSpan.Zero, Period);

            return Task.CompletedTask;
        }

        public abstract Task OnTimerElapsedAsync(AppTimerState appTimerState, CancellationToken cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
