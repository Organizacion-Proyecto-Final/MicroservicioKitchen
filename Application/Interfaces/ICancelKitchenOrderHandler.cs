namespace Application.Interfaces;

public interface ICancelKitchenOrderHandler
{
    Task ExecuteAsync(Guid kitchenOrderId, CancellationToken cancellationToken = default);
    Task ExecuteByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
