using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class KitchenOrchestratorRepository : IKitchenOrchestratorRepository
{
    private readonly ApplicationDbContext _context;

    public KitchenOrchestratorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetMaxConcurrentDishesAsync(CancellationToken cancellationToken = default)
    {
        var config = await _context.KitchenConfigurations.FirstOrDefaultAsync(cancellationToken);
        return config?.MaxConcurrentDishes ?? 10;
    }

    public async Task<List<KitchenOrderItem>> GetFlatActiveQueueAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _context.KitchenOrderItems
            .Include(i => i.Order)
            .Where(i => i.Status == ItemStatus.Pending || i.Status == ItemStatus.Preparing)
            .OrderBy(i => i.StartTime)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateMaxConcurrentDishesAsync(int maxConcurrentDishes, CancellationToken cancellationToken = default)
    {
        var configuration = await _context.KitchenConfigurations.FirstOrDefaultAsync(cancellationToken);

        if (configuration == null)
            throw new NotFoundException("KitchenConfiguration", "default");

        configuration.MaxConcurrentDishes = maxConcurrentDishes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
