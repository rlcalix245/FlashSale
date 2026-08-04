using Confluent.Kafka;
using FlashSale.Shared.Models;
using FlashSale.Shared.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FlashSale.Consumer
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly KafkaSettings _settings;
        private readonly IBalanceRepository _repositorio;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IOptions<KafkaSettings> settings, IBalanceRepository repositorio, ILogger<KafkaConsumerService> logger)
        {
            _settings = settings.Value;
            _repositorio = repositorio;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_settings.Topic);
            _logger.LogInformation("Consumer suscrito a {Topic} (grupo {Group})", _settings.Topic, _settings.GroupId);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var resultado = consumer.Consume(ct);
                    await ProcesarMensajeAsync(resultado);
                    consumer.Commit(resultado);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning("Mensaje malformado descartado: {Error}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            consumer.Close();
        }

        private async Task ProcesarMensajeAsync(ConsumeResult<string, string> resultado)
        {
            _logger.LogInformation("Mensaje recibido: particion {Partition}, offset {Offset}, key {Key}",
                resultado.Partition.Value, resultado.Offset.Value, resultado.Message.Key);

            IntentoCompra? intento;
            try
            {
                intento = JsonSerializer.Deserialize<IntentoCompra>(resultado.Message.Value);
            }
            catch (JsonException)
            {
                _logger.LogWarning("JSON invalido en particion {Partition}, offset {Offset}: se descarta.",
                    resultado.Partition.Value, resultado.Offset.Value);
                return;
            }

            if (intento is null || string.IsNullOrWhiteSpace(intento.ProductId) || intento.QuantityRequested <= 0)
            {
                _logger.LogWarning("Evento incompleto descartado (offset {Offset}).", resultado.Offset.Value);
                return;
            }

            if (await _repositorio.YaProcesadoAsync(intento.EventId))
            {
                _logger.LogInformation("Evento duplicado ignorado: {EventId}", intento.EventId);
                return;
            }

            try
            {
                await _repositorio.AplicarIntentoAsync(intento);
                await _repositorio.MarcarProcesadoAsync(intento.EventId);
                _logger.LogInformation("Balance actualizado para producto {ProductId}", intento.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar en Mongo para el producto {ProductId}", intento.ProductId);
                throw;
            }
        }
    }
}
