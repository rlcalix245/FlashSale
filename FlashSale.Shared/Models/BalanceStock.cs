using System;
using System.Collections.Generic;
using System.Text;

namespace FlashSale.Shared.Models
{
    public class BalanceStock
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockInicial { get; set; }
        public int IntentosAcumulados { get; set; }
        public int UnidadesSolicitadas { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public int BalanceActual => StockInicial - UnidadesSolicitadas;

        public string Estado
        {
            get
            {
                if (BalanceActual < 0) return "Sobreventa";
                if (StockInicial > 0 && BalanceActual <= StockInicial * 0.1) return "Riesgo";
                return "Normal";
            }
        }
    }
}
