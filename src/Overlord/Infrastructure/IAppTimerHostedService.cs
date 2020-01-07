using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Overlord.Infrastructure
{
    public interface IAppTimerHostedService : IHostedService
    {
        TimeSpan Period { get; }

        Task OnTimerElapsedAsync(AppTimerState appTimerState, CancellationToken cancellationToken);
    }
}
