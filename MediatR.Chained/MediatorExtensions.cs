namespace MediatR.Chained;

public static class MediatorExtensions
{
    public static IMediatorChain<TNext> Add<TNext>(this IMediator mediator, IRequest<TNext> request, Func<TNext, bool> failCondition)
    {
        MediatorChain mediatorChain = new(mediator, []);
        return mediatorChain.Add(request, failCondition);
    }
}
