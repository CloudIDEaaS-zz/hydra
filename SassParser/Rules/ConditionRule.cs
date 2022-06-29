
namespace SassParser
{
    internal abstract class ConditionRule : GroupingRule
    {
        internal ConditionRule(RuleType type, Token token, StylesheetParser parser) : base(type, token, parser)
        {
        }
    }
}