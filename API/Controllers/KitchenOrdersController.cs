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
        private readonly IKitchenOrderRepository _repository;
        public KitchenOrdersController(ICreateKitchenOrderHandler createHandler, IKitchenOrderRepository repository)
        {
            _createHandler = createHandler;
            _repository = repository;
        }




        // crea la orden en kitchen  
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKitchenOrderCommand command)
        {
            var order = await _createHandler.CreateKitchenOrder(command);
            return Ok(order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                return NotFound();

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
