namespace MediatR.Chained;

/// <summary>
/// Represents a mediator chain that allows adding requests and sending them asynchronously.
/// </summary>
public interface IMediatorChain
{

    public bool Failed { get; }

    /// <summary>
    /// Adds a request to the mediator chain.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request, Func<TNext, bool> failCondition);

    /// <summary>
    /// Adds a request to the mediator chain using a factory function.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous request in the chain.</typeparam>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The factory function that creates the request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request, Func<TNext, bool> failCondition);

    /// <summary>
    /// Sends the requests in the mediator chain asynchronously and returns the response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the response of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse?> SendAsync<TResponse>(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the requests in the mediator chain asynchronously and returns the response as an object.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the response as an object.</returns>
    Task<object?> SendAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a mediator chain with a specific result type.
/// </summary>
/// <typeparam name="TPrevious">The type of the result of the mediator chain.</typeparam>
public interface IMediatorChain<TPrevious> : IMediatorChain
{
    /// <summary>
    /// Adds a request to the mediator chain using a factory function.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The factory function that creates the request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    IMediatorChain<TNext> Add<TNext>(Func<TPrevious, IRequest<TNext>> request, Func<TNext, bool> failCondition);

    /// <summary>
    /// Adds a condition to the mediator chain that will cause the chain to fail if the condition is met.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>The mediator chain with the added condition.</returns>
    IMediatorChain<TPrevious> FailWhen(Func<TPrevious, bool> predicate);
}
