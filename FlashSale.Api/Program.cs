using FlashSale.Api;
using FlashSale.Api.Repositories;
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IBalanceRepository, MongoBalanceReadRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.Run();