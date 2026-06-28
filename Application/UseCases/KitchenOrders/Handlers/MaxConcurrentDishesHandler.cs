using Application.Interfaces;
using Application.UseCases.KitchenOrders.Comands;
using Domain.Exceptions;

namespace Application.UseCases.KitchenOrders.Handlers;

public sealed class MaxConcurrentDishesHandler : IMaxConcurrentDishesHandler
{
    private readonly IKitchenOrchestratorRepository _orchestratorRepository;
    private readonly IKitchenOrchestrator _orchestrator;

    public MaxConcurrentDishesHandler(
        IKitchenOrchestratorRepository orchestratorRepository,
        IKitchenOrchestrator orchestrator)
    {
        _orchestratorRepository = orchestratorRepository;
        _orchestrator = orchestrator;
    }

    public async Task ExecuteAsync(UpdateMaxConcurrentDishesCommand command, CancellationToken cancellationToken = default)
    {
        if (command.MaxConcurrentDishes <= 0)
        {
            throw new ValidationExceptions(new Dictionary<string, string[]>
            {
                { "maxConcurrentDishes", new[] { "MaxConcurrentDishes debe ser mayor a 0." } }
            });
        }

        await _orchestratorRepository.UpdateMaxConcurrentDishesAsync(command.MaxConcurrentDishes, cancellationToken);

        await _orchestrator.ScheduleAsync(cancellationToken);
    }
}
