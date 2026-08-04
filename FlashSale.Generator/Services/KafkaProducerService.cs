using System.Text.Json;
using Confluent.Kafka;
using FlashSale.Shared.Models;
using Microsoft.Extensions.Options;

namespace FlashSale.Generator.Services;
public class KafkaProducerService : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IOptions<KafkaSettings> settings, ILogger<KafkaProducerService> logger)
    {
        _topic = settings.Value.Topic;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            Acks = Acks.All,
            LingerMs = 5,
            CompressionType = CompressionType.Lz4
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<int> EnviarAsync(IntentoCompra intento)
    {
        var payload = JsonSerializer.Serialize(intento);
        var result = await _producer.ProduceAsync(_topic, new Message<string, string>
        {
            Key = intento.ProductId,
            Value = payload
        });
        return result.Partition.Value;
    }

    public async Task<int> EnviarLoteAsync(IEnumerable<IntentoCompra> intentos)
    {
        var enviados = 0;
        foreach (var intento in intentos)
        {
            var payload = JsonSerializer.Serialize(intento);
            _producer.Produce(_topic, new Message<string, string> { Key = intento.ProductId, Value = payload },
                deliveryReport =>
                {
                    if (deliveryReport.Error.IsError)
                        _logger.LogWarning("Fallo al entregar evento {EventId}: {Error}", intento.EventId, deliveryReport.Error.Reason);
                });
            enviados++;
        }
        _producer.Flush(TimeSpan.FromSeconds(10));
        await Task.CompletedTask;
        return enviados;
    }

    public void Dispose() => _producer.Dispose();
}