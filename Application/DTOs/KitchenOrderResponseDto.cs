using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class KitchenOrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int TableNumber { get; set; }
        public string WaiterName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EstimatedFinishTime { get; set; }
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public List<KitchenOrderItemResponseDto> Items { get; set; } = new();
    }

    public class KitchenOrderItemResponseDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int EstimatedTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Status { get; set; }
        public int PriorityScore { get; set; }
        public string Notes { get; set; }
    }
}
