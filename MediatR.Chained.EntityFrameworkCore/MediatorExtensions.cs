using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MediatR.Chained;

public static class MediatorExtensions
{
    /// <summary>
    /// Sends a request through the mediator chain within the specified database transaction.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="mediatorChain">The mediator chain.</param>
    /// <param name="transaction">The database transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<TResponse?> SendAsync<TResponse>(this IMediatorChain mediatorChain, IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        string savePointName = $"{nameof(IMediatorChain)}_{Guid.NewGuid()}";

        await transaction.CreateSavepointAsync(savePointName, cancellationToken);

        TResponse? response;

        try
        {
            response = await mediatorChain.SendAsync<TResponse>(cancellationToken);

            if (mediatorChain.Failed)
            {
                await transaction.RollbackToSavepointAsync(savePointName, cancellationToken);
                return response;
            }
        }
        catch
        {
            await transaction.RollbackToSavepointAsync(savePointName, cancellationToken);
            throw;
        }

        await transaction.CommitAsync(cancellationToken);

        return response;
    }

    /// <summary>
    /// Sends a request through the mediator chain within a database transaction created from the specified database facade.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="mediatorChain">The mediator chain.</param>
    /// <param name="databaseFacade">The database facade.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<TResponse?> SendAsync<TResponse>(this IMediatorChain mediatorChain, DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = databaseFacade.BeginTransaction();
        return await SendAsync<TResponse>(mediatorChain, transaction, cancellationToken);
    }

    /// <summary>
    /// Sends a request through the mediator chain within a database transaction created from the specified DbContext.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="mediatorChain">The mediator chain.</param>
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<TResponse?> SendAsync<TResponse>(this IMediatorChain mediatorChain, DbContext dbContext, CancellationToken cancellationToken = default)
        => await mediatorChain.SendAsync<TResponse>(dbContext.Database, cancellationToken: cancellationToken);

    /// <summary>
    /// Sends a request through the mediator chain within a database transaction created from the specified IDbContextFactory.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="mediatorChain">The mediator chain.</param>
    /// <param name="factory">The IDbContextFactory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<TResponse?> SendAsync<TResponse, TDbContext>(this IMediatorChain mediatorChain, IDbContextFactory<TDbContext> factory, CancellationToken cancellationToken = default) where TDbContext : DbContext
       => await mediatorChain.SendAsync<TResponse>((await factory.CreateDbContextAsync(cancellationToken)).Database, cancellationToken);
}