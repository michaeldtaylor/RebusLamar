using System;
using System.ComponentModel.DataAnnotations;

namespace Overlord.Domain.Messages
{
    public class IncomingOrder
    {
        [Key]
        public string OrderId { get; set; }

        public string Sender { get; set; }

        public DateTime EndDate { get; set; }

        public string ProductOptionName { get; set; }
    }
}
