using MediatR;

namespace MediatR.Chained;

public interface IMediatorChain
{
    IMediatorChain<TNext> Add<TNext>(IRequest<TNext> request);
    IMediatorChain<TNext> AddRequest<TPrevious, TNext>(Func<TPrevious, IRequest<TNext>> request);
    Task SendAsync(CancellationToken cancellationToken = default);
}

public interface IMediatorChain<T> : IMediatorChain
{
    IMediatorChain<TNext> Add<TNext>(Func<T, IRequest<TNext>> request);
}