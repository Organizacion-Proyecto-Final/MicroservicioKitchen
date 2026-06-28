namespace Application.DTOs;

public sealed class CreateKitchenOrderItemDto
{
    public Guid OrderItemId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int Quantity { get; init; }
    public decimal FactorMultiplierTime { get; init; }
    public string? Notes { get; init; }
}
