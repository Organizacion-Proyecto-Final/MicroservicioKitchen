using Application.DTOs;
using Application.UseCases.KitchenOrders.Comands;

namespace Application.Interfaces;

public interface ICreateKitchenOrderHandler
{
    Task<CreateKitchenOrderResponseDto> CreateKitchenOrder(CreateKitchenOrderCommand command, CancellationToken cancellationToken = default);
}
