namespace SassParser
{
    public interface ISupportsRule : IConditionRule
    {
        IConditionFunction Condition { get; }
    }
}