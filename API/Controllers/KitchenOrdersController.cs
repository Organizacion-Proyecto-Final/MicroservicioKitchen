using Application.Interfaces;
using Application.UseCases.KitchenOrders.Comands;
using Application.UseCases.KitchenOrders.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenOrdersController : ControllerBase
    {
        private readonly ICreateKitchenOrderHandler _createHandler;

        public KitchenOrdersController(ICreateKitchenOrderHandler createHandler)
        {
            _createHandler = createHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKitchenOrderCommand command)
        {
            var order = await _createHandler.CreateKitchenOrder(command);
            return Ok(order);
        }
    }
}
