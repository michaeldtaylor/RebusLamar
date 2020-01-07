using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;
using Overlord.Domain.Events;
using Rebus.Bus;

namespace Overlord
{
    public class OverlordService : IHostedService, IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IBus _bus;

        public static readonly Type[] HandledEventTypes =
        {
            // Orders
            typeof(OrderReceived),
            typeof(ProductVariantGenerated),

            typeof(ProductVariantGenerationFailed),
            typeof(OrderFailed)
        };

        public OverlordService(IBus bus)
        {
            _bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Overlord Starting");

            Task.WaitAll(HandledEventTypes.Select(e => _bus.Subscribe(e)).ToArray());

            Log.Debug("Overlord Started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Overlord Stopping");

            Task.WaitAll(HandledEventTypes.Select(e => _bus.Unsubscribe(e)).ToArray());

            Log.Debug("Overlord Stoppd");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
