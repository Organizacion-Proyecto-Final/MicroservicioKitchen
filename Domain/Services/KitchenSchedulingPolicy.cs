using Domain.Entities;

namespace Domain.Services;

public sealed class KitchenSchedulingPolicy
{
    public void Schedule(KitchenOrder order)
    {
        if (order.Items.Count == 0)
            return;

        var longestEstimated = order.Items.Max(item => item.ComputeEstimatedTime());
        var targetFinish = DateTime.UtcNow.AddMinutes(longestEstimated);

        foreach (var item in order.Items)
            item.Schedule(targetFinish);
    }
}
