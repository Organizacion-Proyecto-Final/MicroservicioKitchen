using Application.UseCases.KitchenOrders.Comands;

namespace Application.Interfaces;

public interface IMaxConcurrentDishesHandler
{
    Task ExecuteAsync(UpdateMaxConcurrentDishesCommand command, CancellationToken cancellationToken = default);
}
