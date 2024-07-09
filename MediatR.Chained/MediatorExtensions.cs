namespace MediatR.Chained;

public static class MediatorExtensions
{
    /// <summary>
    /// Adds a request to the mediator chain.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="request">The request to be added to the chain.</param>
    /// <returns>The mediator chain with the added request.</returns>
    public static IMediatorChain<TNext> Add<TNext>(this IMediator mediator, IRequest<TNext> request)
    {
        MediatorChain mediatorChain = new(mediator, []);
        return mediatorChain.Add(request);
    }
}
