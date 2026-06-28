using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class KitchenOrderRepository : IKitchenOrderRepository
{
    private readonly ApplicationDbContext _context;

    public KitchenOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KitchenOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrders
            .Include(k => k.Items)
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<KitchenOrder?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.OrderId == orderId, cancellationToken);
    }

    public async Task<KitchenOrder> CreateAsync(KitchenOrder order, CancellationToken cancellationToken = default)
    {
        await _context.KitchenOrders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<KitchenOrder> UpdateAsync(KitchenOrder order, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.KitchenOrders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);
            return order;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }

    public async Task<List<KitchenOrder>> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrders
            .Include(o => o.Items)
            .Where(o => o.Status == OrderStatus.Preparing)
            .ToListAsync(cancellationToken);
    }

    public async Task<KitchenOrder?> GetNextWaitingOrderAsync(CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrders
            .Include(o => o.Items)
            .Where(o => o.Status == OrderStatus.Pending)
            .OrderBy(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<KitchenOrder?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
