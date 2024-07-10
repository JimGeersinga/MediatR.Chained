namespace MediatR.Chained;

public static class MediatorExtensions
{
    public static IMediatorChain<BaseType> Chain<BaseType>(this IMediator mediator)
    {
        return new MediatorChain<BaseType>(mediator, []);
    }

    public static IMediatorChain<object, TNext> Add<TNext>(this IMediator mediator, IRequest<TNext> request)
    {
        MediatorChain<object> mediatorChain = new(mediator, []);
        return mediatorChain.Add(request);
    }
}
