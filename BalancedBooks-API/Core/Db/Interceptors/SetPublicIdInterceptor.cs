using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NanoidDotNet;

namespace BalancedBooks_API.Core.Db.Interceptors;

internal interface IPublicEntity
{
    public string PublicId { get; }
}

public sealed class SetPublicIdInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is not null)
        {
            SetPublicUrl(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async void SetPublicUrl(DbContext context)
    {
        var publicEntities = context.ChangeTracker.Entries<IPublicEntity>().ToList();
        var entitiesToAdd = publicEntities.Where(entry => entry.State == EntityState.Added);

        foreach (var entry in entitiesToAdd)
        {
            var publicId =
                await Nanoid.GenerateAsync("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz-", 12);

            entry.Property(x => x.PublicId).CurrentValue = publicId;
        }
    }
}