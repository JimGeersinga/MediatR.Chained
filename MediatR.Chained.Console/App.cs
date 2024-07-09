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

        await mediator
            .Add(new TestCommand1("Hello"))
            .Add(x => new TestCommand2(x, "World"))
            .Add(x => new TestCommand3(x.Item1, x.Item2, "Again"))
            .SendAsync();
    }
}

public record TestCommand1(string Param1) : IRequest<string>;
public record TestCommand2(string Param1, string Param2) : IRequest<(string, string)>;
public record TestCommand3(string Param1, string Param2, string Param3) : IRequest<(string, string, string)>;

internal class Command1Handler : IRequestHandler<TestCommand1, string>
{
    public Task<string> Handle(TestCommand1 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command1Handler: {request.Param1}");
        return Task.FromResult(request.Param1);
    }
}

internal class Command2Handler : IRequestHandler<TestCommand2, (string, string)>
{
    public Task<(string, string)> Handle(TestCommand2 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command2Handler: {request.Param1}, {request.Param2}");
        return Task.FromResult((request.Param1, request.Param2));
    }
}

internal class Command3Handler : IRequestHandler<TestCommand3, (string, string, string)>
{
    public Task<(string, string, string)> Handle(TestCommand3 request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Command3Handler: {request.Param1}, {request.Param2}, {request.Param3}");
        return Task.FromResult((request.Param1, request.Param2, request.Param3));
    }
}
