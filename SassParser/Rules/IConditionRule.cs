namespace SassParser
{
    public interface IConditionRule : IGroupingRule
    {
        string ConditionText { get; set; }
    }
}