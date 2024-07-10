using ErrorOr;

namespace MediatR.Chained;

/// <summary>
/// The `MediatorChain` class is used to create and execute a chain of commands or requests sequentially.
/// It implements the `IMediatorChain` interface.
/// </summary>
/// <param name="mediator">The mediator instance.</param>
/// <param name="steps">The list of functions representing the steps in the chain.</param>
public class MediatorChain(IMediator mediator, List<Func<object, Task<object?>>> steps) : IMediatorChain
{
    /// <summary>
    /// Adds a request to the chain.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The request to be added to the chain.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request) where TNext : IErrorOr
    {
        steps.Add(async _ => await mediator.Send(request));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    /// <summary>
    /// Adds a request to the chain with a previous result.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous result.</typeparam>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The function that creates the request based on the previous result.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request) where TNext : IErrorOr
    {
        steps.Add(async prevResult => await mediator.Send(request((TPrevious)prevResult)));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    /// <summary>
    /// Sends the chain of requests sequentially.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    public async Task<IErrorOr> SendAsync(CancellationToken cancellationToken = default)
    {
        IErrorOr? result = null;
        foreach (Func<object, Task<object?>> step in steps)
        {
            object? response = await step(result!);
            if (response is IErrorOr errorOr)
            {
                if (errorOr.IsError)
                {
                    return errorOr;
                }

                result = errorOr;
            }
        }

        return result;
    }
}

public class MediatorChain<T>(IMediator mediator, List<Func<object, Task<object?>>> steps) 
    : MediatorChain(mediator, steps), IMediatorChain<T> where T : IErrorOr
{
    /// <summary>
    /// Adds a request to the chain with a previous result.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The function that creates the request based on the previous result.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TNext>(Func<T, IRequest<TNext>> request) where TNext : IErrorOr
    {
        steps.Add(async prevResult => await mediator.Send(request((T)prevResult)));
        return new MediatorChain<TNext>(mediator, steps);
    }

    /// <summary>
    /// Adds a condition to the chain that fails when the specified predicate returns true.
    /// </summary>
    /// <param name="predicate">The predicate function that determines whether the condition is met.</param>
    /// <returns>The current mediator chain with the added condition.</returns>
    public IMediatorChain<T> FailWhen(Func<T, bool> predicate)
    {
        steps.Add(async prevResult => await Task.FromResult(predicate((T)prevResult)));
        return new MediatorChain<T>(mediator, steps);
    }
}
