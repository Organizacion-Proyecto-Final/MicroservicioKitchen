using Domain.Entities;

namespace Application.Interfaces;

public interface IKitchenOrchestratorRepository
{
    Task<int> GetMaxConcurrentDishesAsync(CancellationToken cancellationToken = default);
    Task UpdateMaxConcurrentDishesAsync(int maxConcurrentDishes, CancellationToken cancellationToken = default);
}
