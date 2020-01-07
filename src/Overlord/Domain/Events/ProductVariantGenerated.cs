using System;

namespace Overlord.Domain.Events
{
    public class ProductVariantGenerated
    {
        public string OrderId { get; set; }

        public string Identifier { get; set; }

        public TimeSpan? GeneratedTimeSpan { get; set; }
    }
}
