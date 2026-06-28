namespace Application.Interfaces;

public interface IOrderServiceClient
{
    Task NotifyOrderReadyAsync(Guid orderId, CancellationToken cancellationToken = default);
}
