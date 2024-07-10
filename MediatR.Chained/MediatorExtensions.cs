namespace MediatR.Chained;

public static class MediatorExtensions
{
    public static IMediatorChain<TNext> Add<TNext>(this IMediator mediator, IRequest<TNext> request)
    {
        MediatorChain mediatorChain = new(mediator, []);
        return mediatorChain.Add(request);
    }
}
