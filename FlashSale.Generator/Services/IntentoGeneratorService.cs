using FlashSale.Shared.Models;

namespace FlashSale.Generator.Services
{
    public record ProductoCatalogo(string ProductId, string Nombre, string Categoria, decimal Precio, double Peso);

    public class IntentoGeneratorService
    {
        private readonly Random _random = new();

        private readonly List<ProductoCatalogo> _catalogo = new()
    {
        new("P001", "Laptop Gamer",          "Computadoras", 32499.00m, 5.0),
        new("P002", "Audifonos Bluetooth",   "Audio",         1499.00m, 3.0),
        new("P003", "Teclado Mecanico",      "Perifericos",   2249.00m, 4.0),
        new("P004", "Mouse Inalambrico",     "Perifericos",    749.00m, 2.0),
        new("P005", "Monitor 24\"",          "Monitores",     4749.00m, 2.5),
        new("P006", "Silla Gamer",           "Mobiliario",    5749.00m, 1.0),
        new("P007", "Camiseta Edicion Ltda", "Ropa",           499.00m, 0.5),
        new("P008", "Cargador USB-C",        "Accesorios",     624.00m, 1.5),
        new("P009", "Webcam HD",             "Perifericos",   1249.00m, 0.8),
        new("P010", "Mochila Laptop",        "Accesorios",     999.00m, 0.6),
    };

        public IntentoCompra GenerarUno(string? productoIdForzado = null, string? clienteId = null)
        {
            var producto = productoIdForzado is not null
                ? _catalogo.First(p => p.ProductId == productoIdForzado)
                : ElegirProductoPonderado();

            return new IntentoCompra
            {
                ProductId = producto.ProductId,
                ProductName = producto.Nombre,
                Category = producto.Categoria,
                ClientId = clienteId ?? $"cliente_{_random.Next(10000, 99999)}",
                QuantityRequested = _random.Next(1, 4),
                UnitPrice = producto.Precio,
                Timestamp = DateTime.UtcNow
            };
        }
        public List<IntentoCompra> GenerarLote(int cantidad, string? productoViral = null)
        {
            var lote = new List<IntentoCompra>(cantidad);
            for (var i = 0; i < cantidad; i++)
            {
                var forzarViral = productoViral is not null && _random.NextDouble() < 0.7;
                lote.Add(GenerarUno(forzarViral ? productoViral : null));
            }
            return lote;
        }

        public IReadOnlyList<ProductoCatalogo> Catalogo => _catalogo;

        private ProductoCatalogo ElegirProductoPonderado()
        {
            var pesoTotal = _catalogo.Sum(p => p.Peso);
            var punto = _random.NextDouble() * pesoTotal;
            var acumulado = 0.0;
            foreach (var producto in _catalogo)
            {
                acumulado += producto.Peso;
                if (punto <= acumulado) return producto;
            }
            return _catalogo[^1];
        }
    }
}
