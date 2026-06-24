using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.KitchenOrders.Comands;
using Application.UseCases.KitchenOrders.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/kitchenOrders")]
    public class KitchenOrdersController : ControllerBase
    {
        private readonly ICreateKitchenOrderHandler _createHandler;

        public KitchenOrdersController(ICreateKitchenOrderHandler createHandler)
        {
            _createHandler = createHandler;
        }




        // crea la orden en kitchen  
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKitchenOrderCommand command)
        {
            var order = await _createHandler.CreateKitchenOrder(command);
            return Ok(order);
        }




        // devolver la lista de platos al front 
        [HttpGet("queue")]
        public async Task<IActionResult> GetQueue()
        {
            // repensar para la lista que debe devolver al front 

            return Ok();
        }


        // para marcar plato como ya finalizado 
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("ok");
        }

    }
}
