using FlashSale.Consumer;
using FlashSale.Consumer.Repositories;
using FlashSale.Shared.Models;
using FlashSale.Shared.Repositories;
using MongoDB.Bson.Serialization;

if (!BsonClassMap.IsClassMapRegistered(typeof(BalanceStock)))
{
    BsonClassMap.RegisterClassMap<BalanceStock>(cm =>
    {
        cm.AutoMap();
        cm.SetIgnoreExtraElements(true);
    });
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IBalanceRepository, MongoBalanceRepository>();
builder.Services.AddHostedService<KafkaConsumerService>();

var host = builder.Build();
host.Run();