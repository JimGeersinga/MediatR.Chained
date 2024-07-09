using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MediatR.Chained;

public static class MediatorExtensions
{
    /// <summary>
    /// Sends the specified mediator chain asynchronously within the context of a database transaction.
    /// </summary>
    /// <param name="mediatorChain">The mediator chain to send.</param>
    /// <param name="transaction">The database transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Sends the specified mediator chain asynchronously within the context of a database facade.
    /// </summary>
    /// <param name="mediatorChain">The mediator chain to send.</param>
    /// <param name="databaseFacade">The database facade.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendAsync(this IMediatorChain mediatorChain, DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = databaseFacade.BeginTransaction();
        await SendAsync(mediatorChain, transaction, cancellationToken);
    }

    /// <summary>
    /// Sends the specified mediator chain asynchronously within the context of a DbContext.
    /// </summary>
    /// <param name="mediatorChain">The mediator chain to send.</param>
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendAsync(this IMediatorChain mediatorChain, DbContext dbContext, CancellationToken cancellationToken = default)
        => await mediatorChain.SendAsync(dbContext.Database, cancellationToken: cancellationToken);

    /// <summary>
    /// Sends the specified mediator chain asynchronously within the context of a DbContextFactory.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="mediatorChain">The mediator chain to send.</param>
    /// <param name="factory">The DbContextFactory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendAsync<TDbContext>(this IMediatorChain mediatorChain, IDbContextFactory<TDbContext> factory, CancellationToken cancellationToken = default) where TDbContext : DbContext
       => await mediatorChain.SendAsync((await factory.CreateDbContextAsync(cancellationToken)).Database, cancellationToken);
}