using FlashSale.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashSale.Shared.Repositories
{
    public interface IBalanceRepository
    {
        Task AplicarIntentoAsync(IntentoCompra intento, int stockInicialPorDefecto = 100);

        Task<IEnumerable<BalanceStock>> ObtenerTodosAsync();
        Task<BalanceStock?> ObtenerPorProductoAsync(string productId);

        Task<bool> YaProcesadoAsync(string eventId);
        Task MarcarProcesadoAsync(string eventId);
    }
}
