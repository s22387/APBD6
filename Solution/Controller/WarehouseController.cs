using Microsoft.AspNetCore.Mvc;
using Solution.DbService.Interfaces;
using Solution.DTO;

namespace Solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IDbService _dbService;

        public WarehousesController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(CreateProductDto dto)
        {
            var productExists = await _dbService.ProductExists(dto.IdProduct);

            if (productExists is false)
                return BadRequest("Item doesn't exist");

            var warehouseExists = await _dbService.WarehouseExists(dto.IdWarehouse);

            if (warehouseExists is false)
                return BadRequest("Store doesn't exist");

            var orderId = await _dbService.GetOrderId(dto.IdProduct, dto.Amount, dto.CreatedAt);

            if (orderId == null)
                return BadRequest("No orders in DB");

            var orderIsCompleted = await _dbService.OrderIsCompleted(orderId.Value);

            if (orderIsCompleted)
                return BadRequest("Order was completed");

            var insertedId = await _dbService.CompleteOrder(dto, orderId.Value);

            return CreatedAtAction(null, new { Id = insertedId });
        }
    }
}