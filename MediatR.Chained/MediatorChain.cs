namespace MediatR.Chained;

public class MediatorChain(IMediator mediator, List<Func<object, Task<object?>>> steps) : IMediatorChain
{
    public IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request)
    {
        steps.Add(async _ => await mediator.Send(request));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    public IMediatorChain<TNext> AddRequest<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request)
    {
        steps.Add(async prevResult => await mediator.Send(request((TPrevious)prevResult)));
        return new MediatorChain<TNext>(mediator, steps!);
    }

    public async Task SendAsync(CancellationToken cancellationToken = default)
    {
        object? result = null;
        foreach (Func<object, Task<object?>> step in steps)
        {
            result = await step(result!);
        }
    }
}

public class MediatorChain<T>(IMediator mediator, List<Func<object, Task<object?>>> steps) : MediatorChain(mediator, steps), IMediatorChain<T>
{
    public IMediatorChain<TNext> Add<TNext>(Func<T, IRequest<TNext>> request)
    {
        steps.Add(async prevResult => await mediator.Send(request((T)prevResult)));
        return new MediatorChain<TNext>(mediator, steps);
    }
}