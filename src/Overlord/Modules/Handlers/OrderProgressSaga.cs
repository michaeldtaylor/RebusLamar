using System.Threading.Tasks;
using NLog;
using Overlord.Domain.Events;
using Overlord.Domain.Messages;
using Rebus.Bus;
using Rebus.Handlers;

namespace Overlord.Modules.Handlers
{
    // This is usually a Saga - just to rule that out it is a simple handler
    public class OrderProgressSaga : IHandleMessages<OrderReceived>, IHandleMessages<ProductVariantGenerationFailed>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IBus _bus;

        public OrderProgressSaga(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(OrderReceived message)
        {
            await _bus.SendLocal(new GenerateProductVariant
            {
                OrderId = message.OrderId,
                EndDate = message.EndDate,
                ProductVariant = message.ProductOption
            });
        }

        public Task Handle(ProductVariantGenerationFailed message)
        {
            Log.Info("Message failed. order_id={0}", message.OrderId);

            return Task.CompletedTask;
        }
    }
}
