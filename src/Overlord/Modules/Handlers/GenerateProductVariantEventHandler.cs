using System.Threading.Tasks;
using NLog;
using Overlord.Domain.Events;
using Overlord.Domain.Messages;
using Overlord.Other;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using ProductVariantGenerationException = Overlord.Domain.ProductVariantGenerationException;

namespace Overlord.Modules.Handlers
{
    public class GenerateAbCoreCouProductVariantEventHandler :
        IHandleMessages<GenerateProductVariant>,
        IHandleMessages<IFailed<GenerateProductVariant>>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IBus _bus;
        private readonly ISimpleThing _simpleThing;

        public GenerateAbCoreCouProductVariantEventHandler(IBus bus, ISimpleThing simpleThing)
        {
            _bus = bus;
            _simpleThing = simpleThing;
        }

        public async Task Handle(GenerateProductVariant message)
        {
            throw new ProductVariantGenerationException();
        }

        public Task Handle(IFailed<GenerateProductVariant> messageWrapper)
        {
            Log.Debug("Product variant generation failed. product_variant_name={0}", messageWrapper.Message.ProductVariant);

            return _bus.Publish(new ProductVariantGenerationFailed
            {
                OrderId = messageWrapper.Message.OrderId,
                ErrorDescription = messageWrapper.ErrorDescription
            });
        }
    }
}
