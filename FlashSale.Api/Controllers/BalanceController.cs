using FlashSale.Shared.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FlashSale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceRepository _repositorio;

        public BalanceController(IBalanceRepository repositorio) => _repositorio = repositorio;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var balances = await _repositorio.ObtenerTodosAsync();
            return Ok(balances);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> ObtenerPorProducto(string productId)
        {
            var balance = await _repositorio.ObtenerPorProductoAsync(productId);
            return balance is null ? NotFound() : Ok(balance);
        }
    }
}
