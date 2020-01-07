using NLog;
using Overlord.Domain.Messages;

namespace Overlord.Modules.IncomingOrders
{
    public class IncomingOrderMessageValidator
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool IsValid(IncomingOrder incomingOrder)
        {
            var orderId = incomingOrder?.OrderId;

            if (string.IsNullOrEmpty(orderId))
            {
                Log.Warn("The incoming order message id was invalid. order_id={0}", orderId ?? "null");

                return false;
            }

            var productOptionName = incomingOrder.ProductOptionName;

            if (!string.IsNullOrEmpty(productOptionName))
            {
                return true;
            }

            Log.Warn("The incoming order message product option was invalid. product_option_name={0}", productOptionName ?? "null");

            return false;
        }
    }
}
