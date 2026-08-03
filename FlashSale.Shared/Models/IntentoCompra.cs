using System;
using System.Collections.Generic;
using System.Text;

namespace FlashSale.Shared.Models
{
    public record IntentoCompra
    {
        public string EventId { get; init; } = Guid.NewGuid().ToString();
        public string ProductId { get; init; } = string.Empty;
        public string ProductName { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public int QuantityRequested { get; init; }
        public decimal UnitPrice { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
