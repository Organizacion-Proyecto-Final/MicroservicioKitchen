namespace Application.DTOs;

public sealed class CreateKitchenOrderResponseDto
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public Guid? KitchenOrderId { get; init; }
}
