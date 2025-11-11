using MetalTrade.DataAccess.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MetalTrade.DataAccess.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        var entries = eventData.Context.ChangeTracker.Entries<ISoftDeletable>().Where(e => e.State == EntityState.Deleted).ToList();
        entries.ForEach(x =>
        {
            x.Entity.IsDeleted = true;
            x.State = EntityState.Modified;
        });
        
        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}