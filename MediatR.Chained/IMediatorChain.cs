using ErrorOr;

namespace MediatR.Chained;

/// <summary>
/// Represents a mediator chain that allows adding requests and sending them asynchronously.
/// </summary>
public interface IMediatorChain
{
    /// <summary>
    /// Adds a request to the mediator chain.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request) where TNext : IErrorOr;

    /// <summary>
    /// Adds a request to the mediator chain using a factory function.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous request in the chain.</typeparam>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The factory function that creates the request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request) where TNext : IErrorOr;

    //IMediatorChain<TPrevious> FailWhen<TPrevious, TNext>(Func<TPrevious, bool> predicate);

    /// <summary>
    /// Sends all the requests in the mediator chain asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<IErrorOr> SendAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a mediator chain with a specific result type.
/// </summary>
/// <typeparam name="T">The type of the result of the mediator chain.</typeparam>
public interface IMediatorChain<T> : IMediatorChain where T : IErrorOr
{
    /// <summary>
    /// Adds a request to the mediator chain using a factory function.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The factory function that creates the request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TNext>(Func<T, IRequest<TNext>> request) where TNext : IErrorOr;

    IMediatorChain<T> FailWhen(Func<T, bool> predicate);
}