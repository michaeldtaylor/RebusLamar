using System;

namespace Overlord.Domain.Events
{
    public class OrderReceived
    {
        public string OrderId { get; set; }

        public DateTime EndDate { get; set; }

        public string ProductOption { get; set; }
    }
}
