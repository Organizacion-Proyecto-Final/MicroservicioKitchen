using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class KitchenOrder
{
    private readonly List<KitchenOrderItem> _items = new();

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid TableId { get; private set; }
    public int TableNumber { get; private set; }
    public Guid WaiterId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime? ActualFinishTime { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public byte[] Version { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<KitchenOrderItem> Items => _items;

    private KitchenOrder() { }

    public static KitchenOrder Create(Guid orderId, Guid tableId, int tableNumber, Guid waiterId)
    {
        return new KitchenOrder
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            TableId = tableId,
            TableNumber = tableNumber,
            WaiterId = waiterId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(KitchenOrderItem item)
    {
        item.AssignToOrder(Id);
        _items.Add(item);
    }

    public void Enqueue()
    {
        Status = OrderStatus.Pending;
        Touch();
    }

    public void StartPreparing()
    {
        Status = OrderStatus.Preparing;
        Touch();
    }

    public void MarkReady()
    {
        if (!_items.All(i => i.IsFinished))
            throw new DomainException("La orden no puede marcarse como lista hasta que todos sus items esten finalizados.");

        Status = OrderStatus.Ready;
        ActualFinishTime = DateTime.UtcNow;
        Touch();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new ConflictException("La orden ya esta cancelada.");

        if (_items.Any(i => i.Status != ItemStatus.Pending))
            throw new ConflictException("No se puede cancelar la orden porque ya hay items en preparacion o finalizados.");

        Status = OrderStatus.Cancelled;
        Touch();

        foreach (var item in _items)
            item.Cancel();
    }

    public int UsedSlots => _items.Count(i => i.IsPreparing);

    private void Touch() => LastUpdatedAt = DateTime.UtcNow;
}
