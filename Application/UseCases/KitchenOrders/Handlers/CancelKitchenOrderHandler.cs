using Application.Interfaces;
using Domain.Exceptions;

namespace Application.UseCases.KitchenOrders.Handlers;

public sealed class CancelKitchenOrderHandler : ICancelKitchenOrderHandler
{
    private readonly IKitchenOrderRepository _repository;
    private readonly IKitchenOrchestrator _orchestrator;

    public CancelKitchenOrderHandler(
        IKitchenOrderRepository repository,
        IKitchenOrchestrator orchestrator)
    {
        _repository = repository;
        _orchestrator = orchestrator;
    }

    public async Task ExecuteAsync(Guid kitchenOrderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(kitchenOrderId, cancellationToken)
            ?? throw new NotFoundException("KitchenOrder", kitchenOrderId);

        order.Cancel();
        await _repository.UpdateAsync(order, cancellationToken);

        await _orchestrator.ScheduleAsync(cancellationToken);
    }

    public async Task ExecuteByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByOrderIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException("KitchenOrder (OrderId)", orderId);

        order.Cancel();
        await _repository.UpdateAsync(order, cancellationToken);

        await _orchestrator.ScheduleAsync(cancellationToken);
    }
}
