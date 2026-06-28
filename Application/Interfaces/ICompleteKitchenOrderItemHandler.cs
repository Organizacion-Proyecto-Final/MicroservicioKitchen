namespace Application.Interfaces;

public interface ICompleteKitchenOrderItemHandler
{
    Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
