using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.KitchenOrders.Commands;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.KitchenOrders.Handlers;

public sealed class CreateKitchenOrderHandler : ICreateKitchenOrderHandler
{
    private const decimal DefaultFactorMultiplierTime = 0.5m;

    private readonly IKitchenOrderRepository _repository;
    private readonly IKitchenOrchestrator _orchestrator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKitchenOrderHandler(
        IKitchenOrderRepository repository,
        IKitchenOrchestrator orchestrator,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _orchestrator = orchestrator;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateKitchenOrderResponseDto> CreateKitchenOrder(CreateKitchenOrderCommand command, CancellationToken cancellationToken = default)
    {
        Validate(command);

        var existing = await _repository.GetByOrderIdAsync(command.OrderId, cancellationToken);
        if (existing is not null)
        {
            return new CreateKitchenOrderResponseDto
            {
                Success = true,
                Message = "La orden ya estaba registrada en cocina.",
                KitchenOrderId = existing.Id
            };
        }

        var order = BuildOrder(command);

        var savedOrder = await _unitOfWork.ExecuteAsync(async () =>
        {
            var created = await _repository.CreateAsync(order, cancellationToken);
            await _orchestrator.EnqueueOrderAsync(created.Id, cancellationToken);
            return created;
        }, cancellationToken);

        return new CreateKitchenOrderResponseDto
        {
            Success = true,
            KitchenOrderId = savedOrder.Id
        };
    }

    private static void Validate(CreateKitchenOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (command.OrderId == Guid.Empty)
            errors["orderId"] = new[] { "El OrderId es obligatorio." };

        if (command.TableNumber <= 0)
            errors["tableNumber"] = new[] { "El numero de mesa debe ser mayor a 0." };

        if (command.Items is null || command.Items.Count == 0)
        {
            errors["items"] = new[] { "La orden debe contener al menos un item." };
        }
        else
        {
            for (var i = 0; i < command.Items.Count; i++)
            {
                var item = command.Items[i];
                var messages = new List<string>();

                if (item.ProductId == Guid.Empty)
                    messages.Add("El ProductId es obligatorio.");
                if (string.IsNullOrWhiteSpace(item.ProductName))
                    messages.Add("El nombre del producto es obligatorio.");
                if (item.DurationMinutes <= 0)
                    messages.Add("El tiempo estimado debe ser mayor a 0 minutos.");
                if (item.Quantity <= 0)
                    messages.Add("La cantidad debe ser mayor a 0.");

                if (messages.Count > 0)
                    errors[$"items[{i}]"] = messages.ToArray();
            }
        }

        if (errors.Count > 0)
            throw new ValidationExceptions(errors);
    }

    private static KitchenOrder BuildOrder(CreateKitchenOrderCommand command)
    {
        var order = KitchenOrder.Create(command.OrderId, command.TableId, command.TableNumber, command.WaiterId);

        foreach (var itemDto in command.Items)
        {
            var factor = itemDto.FactorMultiplierTime > 0 ? itemDto.FactorMultiplierTime : DefaultFactorMultiplierTime;
            order.AddItem(KitchenOrderItem.Create(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.Quantity,
                itemDto.DurationMinutes,
                factor,
                itemDto.Notes));
        }

        return order;
    }
}
