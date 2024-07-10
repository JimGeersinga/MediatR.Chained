namespace MediatR.Chained;

/// <summary>
/// The `MediatorChain` class is used to create and execute a chain of commands or requests sequentially.
/// It implements the `IMediatorChain` interface.
/// </summary>
/// <param name="mediator">The mediator instance.</param>
/// <param name="steps">The list of functions representing the steps in the chain.</param>
internal class MediatorChain(IMediator mediator, List<MediatorChainStep> steps) : IMediatorChain
{
    /// <summary>
    /// Adds a request to the chain.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The request to be added to the chain.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request)
    {
        steps.Add(new(
            MediatorChainStep.StepType.Add,
            _ => request));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    /// <summary>
    /// Adds a request to the chain with a previous result.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous result.</typeparam>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The function that creates the request based on the previous result.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request)
    {
        steps.Add(new(
            MediatorChainStep.StepType.Add,
             prevResult => request((TPrevious)prevResult)));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    /// <summary>
    /// Sends the chain of requests asynchronously and returns the response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse?> SendAsync<TResponse>(CancellationToken cancellationToken = default)
    {
        object? result = default;
        foreach (MediatorChainStep step in steps)
        {
            if(step.Type == MediatorChainStep.StepType.Add)
            {
                object request = step.Predicate(result!);
                result = await mediator.Send(request, cancellationToken);
            }
            else if (step.Type == MediatorChainStep.StepType.FailWhen && (bool?)step.Predicate(result!) == true)
            {
                break;
            }
        }

        return result is TResponse baseType ? baseType : default;
    }

    /// <summary>
    /// Sends the chain of requests asynchronously and returns the response as an object.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response as an object.</returns>
    public async Task<object?> SendAsync(CancellationToken cancellationToken = default)
        => await SendAsync<object?>(cancellationToken);
}

internal class MediatorChain<TPrevious>(IMediator mediator, List<MediatorChainStep> steps)
    : MediatorChain(mediator, steps), IMediatorChain<TPrevious>
{
    /// <summary>
    /// Adds a request to the chain with a previous result.
    /// </summary>
    /// <typeparam name="TNext">The type of the next request in the chain.</typeparam>
    /// <param name="request">The function that creates the request based on the previous result.</param>
    /// <returns>The next mediator chain with the added request.</returns>
    public IMediatorChain<TNext> Add<TNext>(Func<TPrevious, IRequest<TNext>> request)
    {
        steps.Add(new(
            MediatorChainStep.StepType.Add,
            prevResult => request((TPrevious)prevResult)));
        return new MediatorChain<TNext>(mediator, steps);
    }

    /// <summary>
    /// Adds a condition to the chain that fails when the specified predicate returns true.
    /// </summary>
    /// <param name="predicate">The predicate function that determines whether the condition is met.</param>
    /// <returns>The current mediator chain with the added condition.</returns>
    public IMediatorChain<TPrevious> FailWhen(Func<TPrevious, bool> predicate)
    {
        steps.Add(new(
            MediatorChainStep.StepType.FailWhen,
            prevResult => predicate((TPrevious)prevResult)));
        return new MediatorChain<TPrevious>(mediator, steps);
    }
}
