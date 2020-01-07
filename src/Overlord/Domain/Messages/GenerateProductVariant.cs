using System;

namespace Overlord.Domain.Messages
{
    public class GenerateProductVariant
    {
        public string OrderId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ProductVariant { get; set; }
    }
}
