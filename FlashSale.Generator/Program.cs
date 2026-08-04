using FlashSale.Generator;
using FlashSale.Generator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddSingleton<IntentoGeneratorService>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.MapPost("/api/intentos", async (IntentoRequest req, KafkaProducerService producer, IntentoGeneratorService gen) =>
{
    var intento = gen.GenerarUno(req.ProductId, req.ClienteId);
    var particion = await producer.EnviarAsync(intento with { QuantityRequested = req.Cantidad ?? intento.QuantityRequested });
    return Results.Ok(new { intento.EventId, intento.ProductId, particion });
});

app.MapPost("/api/intentos/lote", async (LoteRequest req, KafkaProducerService producer, IntentoGeneratorService gen) =>
{
    var inicio = DateTime.UtcNow;
    var lote = gen.GenerarLote(req.Cantidad, req.ProductoViral);
    var enviados = await producer.EnviarLoteAsync(lote);
    var duracionMs = (DateTime.UtcNow - inicio).TotalMilliseconds;
    var throughput = duracionMs > 0 ? enviados / (duracionMs / 1000.0) : 0;
    return Results.Ok(new { enviados, duracionMs, eventosPorSegundo = Math.Round(throughput, 1) });
});

app.MapGet("/api/catalogo", (IntentoGeneratorService gen) => Results.Ok(gen.Catalogo));

app.Run();

record IntentoRequest(string ProductId, string? ClienteId, int? Cantidad);
record LoteRequest(int Cantidad, string? ProductoViral);