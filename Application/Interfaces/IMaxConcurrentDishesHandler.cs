using Application.UseCases.KitchenOrders.Commands;

namespace Application.Interfaces;

public interface IMaxConcurrentDishesHandler
{
    Task ExecuteAsync(UpdateMaxConcurrentDishesCommand command, CancellationToken cancellationToken = default);
}
