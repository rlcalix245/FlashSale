using FlashSale.Shared.Models;
using FlashSale.Shared.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FlashSale.Consumer.Repositories
{
    public class MongoBalanceRepository : IBalanceRepository
    {
        private readonly IMongoCollection<BalanceStock> _balances;
        private readonly IMongoCollection<BsonDocument> _eventosProcesados;

        public MongoBalanceRepository(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.Database);
            _balances = db.GetCollection<BalanceStock>("balances");
            _eventosProcesados = db.GetCollection<BsonDocument>("eventos_procesados");
        }

        public async Task AplicarIntentoAsync(IntentoCompra intento, int stockInicialPorDefecto = 100)
        {
            var filtro = Builders<BalanceStock>.Filter.Eq(b => b.ProductId, intento.ProductId);
            var existente = await _balances.Find(filtro).FirstOrDefaultAsync();

            if (existente is null)
            {
                var nuevo = new BalanceStock
                {
                    ProductId = intento.ProductId,
                    ProductName = intento.ProductName,
                    Category = intento.Category,
                    StockInicial = stockInicialPorDefecto,
                    IntentosAcumulados = 1,
                    UnidadesSolicitadas = intento.QuantityRequested,
                    LastUpdated = DateTime.UtcNow
                };
                await _balances.InsertOneAsync(nuevo);
                return;
            }

            var update = Builders<BalanceStock>.Update
                .Inc(b => b.IntentosAcumulados, 1)
                .Inc(b => b.UnidadesSolicitadas, intento.QuantityRequested)
                .Set(b => b.LastUpdated, DateTime.UtcNow);

            await _balances.UpdateOneAsync(filtro, update);
        }

        public async Task<IEnumerable<BalanceStock>> ObtenerTodosAsync() =>
            await _balances.Find(FilterDefinition<BalanceStock>.Empty).ToListAsync();

        public async Task<BalanceStock?> ObtenerPorProductoAsync(string productId) =>
            await _balances.Find(b => b.ProductId == productId).FirstOrDefaultAsync();

        public async Task<bool> YaProcesadoAsync(string eventId)
        {
            var count = await _eventosProcesados.CountDocumentsAsync(new BsonDocument("_id", eventId));
            return count > 0;
        }

        public async Task MarcarProcesadoAsync(string eventId)
        {
            try
            {
                await _eventosProcesados.InsertOneAsync(new BsonDocument { { "_id", eventId }, { "procesadoEn", DateTime.UtcNow } });
            }
            catch (MongoWriteException)
            {
               
            }
        }
    }
}
