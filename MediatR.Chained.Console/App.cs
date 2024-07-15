using ErrorOr;

using MediatR;
using MediatR.Chained;

internal class App(IMediator mediator)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Normal:");

        await mediator.Send(new TestCommand1("Hello"));
        await mediator.Send(new TestCommand2("Hello", "World"));
        await mediator.Send(new TestCommand3("Hello", "World", "Again"));

        Console.WriteLine("Chained:");

        IErrorOr? result = await mediator
            .Add(new TestCommand1("Hello"), x => x.IsError)
            .Add(x => new TestCommand2(x.Value, "World"), _ => false)
            .Add(x => new TestCommand3(x.Value.Item1, x.Value.Item2, "Again"), _ => false)
            .SendAsync<IErrorOr>();

        Console.WriteLine(result!.IsError);

        object? result2 = await mediator
            .Add(new TestCommand1("Hello"), _ => false)
            .Add(x => new TestCommand2(x.Value, "World"), _ => false)
            .Add(x => new TestCommand3(x.Value.Item1, x.Value.Item2, "Again"), _ => false)
            .SendAsync();

        Console.WriteLine(result2);
    }
}

public record TestCommand1(string Param1) : IRequest<ErrorOr<string>>;
public record TestCommand2(string Param1, string Param2) : IRequest<ErrorOr<(string, string)>>;
public record TestCommand3(string Param1, string Param2, string Param3) : IRequest<ErrorOr<(string, string, string)>>;

internal class Command1Handler : IRequestHandler<TestCommand1, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(TestCommand1 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command1Handler: {request.Param1}");
        return await Task.FromResult(Error.Failure("test"));
    }
}

internal class Command2Handler : IRequestHandler<TestCommand2, ErrorOr<(string, string)>>
{
    public async Task<ErrorOr<(string, string)>> Handle(TestCommand2 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command2Handler: {request.Param1}, {request.Param2}");
        return await Task.FromResult((request.Param1, request.Param2));
    }
}

internal class Command3Handler : IRequestHandler<TestCommand3, ErrorOr<(string, string, string)>>
{
    public async Task<ErrorOr<(string, string, string)>> Handle(TestCommand3 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command3Handler: {request.Param1}, {request.Param2}, {request.Param3}");
        return await Task.FromResult((request.Param1, request.Param2, request.Param3));
    }
}
