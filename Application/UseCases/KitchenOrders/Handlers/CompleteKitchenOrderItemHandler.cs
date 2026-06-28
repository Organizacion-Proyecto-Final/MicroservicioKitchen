using Application.Interfaces;

namespace Application.UseCases.KitchenOrders.Handlers;

public sealed class CompleteKitchenOrderItemHandler : ICompleteKitchenOrderItemHandler
{
    private readonly IKitchenOrchestrator _kitchenOrchestrator;

    public CompleteKitchenOrderItemHandler(IKitchenOrchestrator kitchenOrchestrator)
    {
        _kitchenOrchestrator = kitchenOrchestrator;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _kitchenOrchestrator.FinishItemAsync(id, cancellationToken);
    }
}
