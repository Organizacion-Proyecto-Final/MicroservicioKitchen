namespace Application.Interfaces;

public interface ICancelKitchenOrderHandler
{
    Task ExecuteAsync(Guid kitchenOrderId, CancellationToken cancellationToken = default);
}
