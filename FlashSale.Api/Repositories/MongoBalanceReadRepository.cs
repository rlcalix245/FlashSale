using FlashSale.Shared.Models;
using FlashSale.Shared.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FlashSale.Api.Repositories
{
    public class MongoBalanceReadRepository : IBalanceRepository
    {
        private readonly IMongoCollection<BalanceStock> _balances;

        public MongoBalanceReadRepository(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.Database);
            _balances = db.GetCollection<BalanceStock>("balances");
        }

        public async Task<IEnumerable<BalanceStock>> ObtenerTodosAsync() =>
            await _balances.Find(FilterDefinition<BalanceStock>.Empty).ToListAsync();

        public async Task<BalanceStock?> ObtenerPorProductoAsync(string productId) =>
            await _balances.Find(b => b.ProductId == productId).FirstOrDefaultAsync();

        public Task AplicarIntentoAsync(IntentoCompra intento, int stockInicialPorDefecto = 100) =>
            throw new NotSupportedException("La Api es de solo lectura; las escrituras las hace FlashSale.Consumer.");

        public Task<bool> YaProcesadoAsync(string eventId) =>
            throw new NotSupportedException("La Api es de solo lectura; las escrituras las hace FlashSale.Consumer.");

        public Task MarcarProcesadoAsync(string eventId) =>
            throw new NotSupportedException("La Api es de solo lectura; las escrituras las hace FlashSale.Consumer.");
    }
}
