namespace Overlord.Domain.Events
{
    public class ProductVariantGenerationFailed : OrderFailedEvent
    {
    }

    public class OrderFailed : OrderFailedEvent
    {
    }

    public abstract class OrderFailedEvent
    {
        public string OrderId { get; set; }

        public string ErrorDescription { get; set; }
    }
}
