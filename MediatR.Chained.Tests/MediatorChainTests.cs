using FluentAssertions;

using Moq;

namespace MediatR.Chained.Tests;

public class MediatorChainTests
{
    [Fact]
    public async Task SendAsync_ShouldExecuteAllStepsInOrder()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<Func<object, Task<object?>>> steps = [];
        MediatorChain<object> chain = new(mediatorMock.Object, steps);

        bool step1Executed = false;
        bool step2Executed = false;
        bool step3Executed = false;

        steps.Add(_ =>
        {
            step1Executed = true;
            return Task.FromResult<object>(null!)!;
        });

        steps.Add(_ =>
        {
            step2Executed = true;
            return Task.FromResult<object>(null!)!;
        });

        steps.Add(_ =>
        {
            step3Executed = true;
            return Task.FromResult<object>(null!)!;
        });

        // Act
        await chain.SendAsync();

        // Assert
        step1Executed.Should().BeTrue();
        step2Executed.Should().BeTrue();
        step3Executed.Should().BeTrue();
    }

    [Fact]
    public async Task Add_ShouldAddStepToChain()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<Func<object, Task<object?>>> steps = [];
        MediatorChain<object> chain = new(mediatorMock.Object, steps);

        TestRequest request = new();

        // Act
        IMediatorChain<object, TestResponse> nextChain = chain.Add(request);

        // Assert
        steps.Should().HaveCount(1);
        steps[0].Should().NotBeNull();
        steps[0].Should().BeOfType<Func<object, Task<object?>>>();

        object? result = await steps[0](null!);
        result.Should().BeNull();

        nextChain.Should().NotBeNull();
        nextChain.Should().BeOfType<MediatorChain<TestResponse>>();
    }

    [Fact]
    public async Task AddRequest_ShouldAddStepToChain()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<Func<object, Task<object?>>> steps = [];
        MediatorChain<object> chain = new(mediatorMock.Object, steps);

        TestRequest request = new();

        // Act
        IMediatorChain<object, TestResponse> nextChain = chain.Add<TestRequest, TestResponse>(prevResult => new TestRequest());

        // Assert
        steps.Should().HaveCount(1);
        steps[0].Should().NotBeNull();
        steps[0].Should().BeOfType<Func<object, Task<object?>>>();

        object? result = await steps[0](null!);
        result.Should().BeNull();

        nextChain.Should().NotBeNull();
        nextChain.Should().BeOfType<MediatorChain<TestResponse>>();
    }

    private class TestRequest : IRequest<TestResponse>;
    private class TestResponse;
}
