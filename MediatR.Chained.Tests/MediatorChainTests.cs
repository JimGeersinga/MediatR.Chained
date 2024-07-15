using FluentAssertions;

using Moq;

namespace MediatR.Chained.Tests;

public class MediatorChainTests
{
    [Fact]
    public void MediatorChain_AddRequest_ShouldAddRequestToChain()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<MediatorChainStep> steps = [];
        MediatorChain chain = new(mediatorMock.Object, steps);

        SampleRequest1 request = new();

        // Act
        IMediatorChain<SampleResponse1> nextChain = chain.Add(request, _ => false);

        // Assert
        steps.Should().HaveCount(1);
        steps[0].Type.Should().Be(MediatorChainStep.StepType.Add);
        steps[0].Predicate.Invoke(null!).Should().Be(request);
        nextChain.Should().BeOfType<MediatorChain<SampleResponse1>>();
    }

    [Fact]
    public void MediatorChain_AddRequestWithPreviousResult_ShouldAddRequestToChainWithPreviousResult()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<MediatorChainStep> steps = [];
        MediatorChain chain = new(mediatorMock.Object, steps);

        SampleRequest1 request1 = new();
        SampleRequest2 request2 = new();
        SampleResponse1 previousResult = new();

        // Act
        IMediatorChain<SampleResponse1> previouseChain = chain.Add(request1, _ => false);

        IMediatorChain<SampleResponse2> nextChain = previouseChain.Add(request2, _ => false);

        // Assert
        steps.Should().HaveCount(2);
        steps[0].Type.Should().Be(MediatorChainStep.StepType.Add);
        steps[1].Predicate(previousResult).Should().Be(request2);
        nextChain.Should().BeOfType<MediatorChain<SampleResponse2>>();
    }

    [Fact]
    public async Task MediatorChain_SendAsync_ShouldSendChainOfRequestsAndReturnResponse()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<MediatorChainStep> steps = [];
        MediatorChain chain = new(mediatorMock.Object, steps);

        SampleRequest1 request1 = new();
        SampleRequest2 request2 = new();
        SampleResponse1 response1 = new();
        SampleResponse2 response2 = new();

        mediatorMock.Setup(x => x.Send((object)request1, It.IsAny<CancellationToken>()))
         .ReturnsAsync(response1);

        mediatorMock.Setup(x => x.Send((object)request2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response2);

        chain.Add(request1, _ => false);
        chain.Add(request2, _ => false);

        // Act
        SampleResponse2? response = await chain.SendAsync<SampleResponse2>();

        // Assert
        response.Should().Be(response2);
        mediatorMock.Verify(x => x.Send((object)request1, It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(x => x.Send((object)request2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MediatorChain_SendAsync_WithCancellation_ShouldSendChainOfRequestsAndReturnResponse()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();
        List<MediatorChainStep> steps = [];
        MediatorChain chain = new(mediatorMock.Object, steps);

        SampleRequest1 request1 = new();
        SampleRequest2 request2 = new();
        SampleResponse1 response1 = new();
        SampleResponse2 response2 = new();

        mediatorMock.Setup(x => x.Send((object)request1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1);

        mediatorMock.Setup(x => x.Send((object)request2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response2);

        chain.Add(request1, _ => false);
        chain.Add(request2, _ => false);

        CancellationToken cancellationToken = new();

        // Act
        SampleResponse2? response = await chain.SendAsync<SampleResponse2>(cancellationToken);

        // Assert
        response.Should().Be(response2);
        mediatorMock.Verify(x => x.Send((object)request1, cancellationToken), Times.Once);
        mediatorMock.Verify(x => x.Send((object)request2, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task MediatorChain_SendAsync_WithFailWhenCondition_ShouldStopChainExecution()
    {
        // Arrange
        Mock<IMediator> mediatorMock = new();

        SampleRequest1 request1 = new();
        SampleRequest2 request2 = new();
        SampleResponse1 response1 = new();
        SampleResponse2 response2 = new();

        mediatorMock.Setup(x => x.Send((object)request1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1);

        IMediatorChain<SampleResponse2> chain = mediatorMock.Object
            .Add(request1, _ => false)
            .FailWhen(_ => true)
            .Add(request2, _ => false);

        // Act
        SampleResponse1? response = await chain.SendAsync<SampleResponse1>();

        // Assert
        response.Should().Be(response1);
        mediatorMock.Verify(x => x.Send((object)request1, It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(x => x.Send((object)request2, It.IsAny<CancellationToken>()), Times.Never);
    }

    private class SampleRequest1 : IRequest<SampleResponse1>;
    private class SampleRequest2 : IRequest<SampleResponse2>;

    private class SampleResponse1;
    private class SampleResponse2;
}
