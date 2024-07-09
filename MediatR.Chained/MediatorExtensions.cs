namespace MediatR.Chained;

public static class MediatorExtensions
{
    public static IMediatorChain Chain(this IMediator mediator)
    {
        return new MediatorChain(mediator, []);
    }
}