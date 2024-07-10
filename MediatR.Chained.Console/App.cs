using ErrorOr;

using MediatR;
using MediatR.Chained;

using Microsoft.EntityFrameworkCore;

internal class App(IMediator mediator)
{
    public async Task RunAsync()
    {
        var dbcontext = new DbContext();


        Console.WriteLine("Normal:");

        dbcontext.Database.BeginTransaction();
        await mediator.Send(new TestCommand1("Hello"));
        await mediator.Send(new TestCommand2("Hello", "World"));
        await mediator.Send(new TestCommand3("Hello", "World", "Again"));
        dbcontext.Database.CommitTransaction();

        Console.WriteLine("Chained:");

        var result = await mediator.Chain<IErrorOr>()
            .Add(new TestCommand1("Hello"))
            .Add(x => new TestCommand2(x.Value, "World"))
            .Add(x => new TestCommand3(x.Value.Item1, x.Value.Item2, "Again"))
            .SendAsync();


        Console.WriteLine(result.IsError);
    }
}

public record TestCommand1(string Param1) : IRequest<IErrorOr<string>>;
public record TestCommand2(string Param1, string Param2) : IRequest<IErrorOr<(string, string)>>;
public record TestCommand3(string Param1, string Param2, string Param3) : IRequest<IErrorOr<(string, string, string)>>;

internal class Command1Handler : IRequestHandler<TestCommand1, IErrorOr<string>>
{
    public Task< IErrorOr<string>> Handle(TestCommand1 request, CancellationToken cancellationToken)
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
