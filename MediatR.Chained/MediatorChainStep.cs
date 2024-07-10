namespace MediatR.Chained;

internal class MediatorChainStep(MediatorChainStep.StepType type, Func<object, object> predicate)
{
    public enum StepType
    {
        Add,
        FailWhen
    }

    public StepType Type { get; } = type;
    public Func<object, object> Predicate { get; } = predicate;
}
