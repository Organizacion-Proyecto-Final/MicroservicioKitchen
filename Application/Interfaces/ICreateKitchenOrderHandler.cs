using Application.DTOs;
using Application.UseCases.KitchenOrders.Commands;

namespace Application.Interfaces;

public interface ICreateKitchenOrderHandler
{
    Task<CreateKitchenOrderResponseDto> CreateKitchenOrder(CreateKitchenOrderCommand command, CancellationToken cancellationToken = default);
}
