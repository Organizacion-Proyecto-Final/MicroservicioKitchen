using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.UseCases.KitchenOrders.Handlers;

public sealed class CancelKitchenOrderHandler : ICancelKitchenOrderHandler
{
    private readonly IKitchenOrderRepository _repository;
    private readonly IKitchenOrderItemRepository _itemRepository;
    private readonly IKitchenOrchestrator _orchestrator;

    public CancelKitchenOrderHandler(
        IKitchenOrderRepository repository,
        IKitchenOrderItemRepository itemRepository,
        IKitchenOrchestrator orchestrator)
    {
        _repository = repository;
        _itemRepository = itemRepository;
        _orchestrator = orchestrator;
    }

    public async Task ExecuteAsync(Guid kitchenOrderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(kitchenOrderId, cancellationToken)
            ?? throw new NotFoundException("KitchenOrder", kitchenOrderId);

        if (order.Status is OrderStatus.Ready or OrderStatus.Cancelled)
            throw new ConflictException($"No se puede cancelar una orden en estado {order.Status}.");

        order.Status = OrderStatus.Cancelled;
        order.LastUpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(order, cancellationToken);

        await _itemRepository.CancelItemsByOrderIdAsync(kitchenOrderId, cancellationToken);

        await _orchestrator.ScheduleAsync(cancellationToken);
    }
}
