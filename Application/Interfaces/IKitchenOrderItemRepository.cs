using Domain.Entities;

namespace Application.Interfaces;

public interface IKitchenOrderItemRepository
{
    Task<KitchenOrderItem?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task UpdateItemAsync(KitchenOrderItem item, CancellationToken cancellationToken = default);
    Task<List<KitchenOrderItem>> GetItemsReadyToCookAsync(CancellationToken cancellationToken = default);
    Task<List<KitchenOrderItem>> GetItemsToWaitingAsync(CancellationToken cancellationToken = default);
    Task<List<KitchenOrderItem>> GetPendingItemsAsync(CancellationToken cancellationToken = default);
    Task CancelItemsByOrderIdAsync(Guid kitchenOrderId, CancellationToken cancellationToken = default);
}
