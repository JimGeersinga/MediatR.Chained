using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MediatR.Chained;

public static class MediatorExtensions
{
    public static async Task SendAsync(this IMediatorChain mediatorChain, IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        string savePointName = $"{nameof(IMediatorChain)}_{Guid.NewGuid()}";

        await transaction.CreateSavepointAsync(savePointName, cancellationToken);

        try
        {
            await mediatorChain.SendAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackToSavepointAsync(savePointName, cancellationToken);
            throw;
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public static async Task SendAsync(this IMediatorChain mediatorChain, DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = databaseFacade.BeginTransaction();
        await SendAsync(mediatorChain, transaction, cancellationToken);
    }

    public static async Task SendAsync(this IMediatorChain mediatorChain, DbContext dbContext, CancellationToken cancellationToken = default)
        => await mediatorChain.SendAsync(dbContext.Database, cancellationToken: cancellationToken);

    public static async Task SendAsync<TDbContext>(this IMediatorChain mediatorChain, IDbContextFactory<TDbContext> factory, CancellationToken cancellationToken = default) where TDbContext : DbContext
       => await mediatorChain.SendAsync((await factory.CreateDbContextAsync(cancellationToken)).Database, cancellationToken);
}