using Application.DTOs;

namespace Application.UseCases.KitchenOrders.Commands;

public sealed class CreateKitchenOrderCommand
{
    public Guid OrderId { get; init; }
    public Guid TableId { get; init; }
    public int TableNumber { get; init; }
    public Guid WaiterId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public List<CreateKitchenOrderItemDto> Items { get; init; } = new();
}
