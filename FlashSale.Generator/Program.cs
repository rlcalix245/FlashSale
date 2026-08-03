var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "FlashSale.Generator funcionando");

app.Run();