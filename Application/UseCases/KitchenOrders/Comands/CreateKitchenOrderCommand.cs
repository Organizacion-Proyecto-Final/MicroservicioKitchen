using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.KitchenOrders.Comands
{
    public class CreateKitchenOrderCommand
    {
        public Guid OrderId { get; set; }
        public int TableNumber { get; set; }
        public string WaiterName { get; set; } = string.Empty;
        public List<CreateKitchenOrderItemDto> Items { get; set; } = new();
    }

    public class CreateKitchenOrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int EstimatedTime { get; set; } // en minutos
        public string Notes { get; set; } = string.Empty;
    }
}
