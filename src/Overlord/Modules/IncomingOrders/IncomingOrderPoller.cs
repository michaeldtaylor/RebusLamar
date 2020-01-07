using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Overlord.Domain.Events;
using Overlord.Domain.Messages;
using Overlord.Infrastructure;
using Rebus.Bus;

namespace Overlord.Modules.IncomingOrders
{
    public class IncomingOrderPoller : AppTimerHostedService
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IBus _bus;
        private readonly IncomingOrderMessageValidator _incomingOrderMessageValidator;
        private bool _hasSentOneMessage;

        public IncomingOrderPoller(
            IBus bus,
            IAppNameProvider appNameProvider,
            IncomingOrderMessageValidator incomingOrderMessageValidator)
            : base(appNameProvider)
        {
            _bus = bus;
            _incomingOrderMessageValidator = incomingOrderMessageValidator;
            _hasSentOneMessage = false;
        }

        public override async Task OnTimerElapsedAsync(AppTimerState appTimerState, CancellationToken cancellationToken)
        {
            Log.Info("Polling incoming order queue. signal_time={0}", DateTime.UtcNow);

            // await _incomingOrderQueue.ProcessMessagesAsync(async incomingOrder =>
            // {
                if (_hasSentOneMessage)
                {
                    return;
                }

                // Usually grabbed from a queue
                var incomingOrder = new IncomingOrder
                {
                    OrderId = Guid.NewGuid().ToString("N"),
                    EndDate = DateTime.Today.Date,
                    ProductOptionName = "Something"
                };
            
                var isValid = _incomingOrderMessageValidator.IsValid(incomingOrder);

                if (isValid)
                {
                    await _bus.SendLocal(new OrderReceived
                    {
                        OrderId = incomingOrder.OrderId,
                        EndDate = incomingOrder.EndDate,
                        ProductOption = incomingOrder.ProductOptionName
                    });

                    _hasSentOneMessage = true;
                }
            // });
        }
    }
}
