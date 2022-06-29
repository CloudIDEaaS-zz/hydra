
namespace SassParser
{
    internal abstract class GroupingRule : Rule, IGroupingRule
    {
        internal GroupingRule(RuleType type, Token token, StylesheetParser parser) : base(type, token, parser)
        {
            Rules = new RuleList(this);
        }

        public RuleList Rules { get; }

        IRuleList IGroupingRule.Rules => Rules;

        public IRule AddNewRule(RuleType ruleType, Token token)
        {
            var rule = Parser.CreateRule(ruleType, token);
            Rules.Add(rule);
            return rule;
        }

        public int Insert(string ruleText, int index)
        {
            var rule = Parser.ParseRule(ruleText);
            Rules.Insert(index, rule);
            return index;
        }

        public void RemoveAt(int index)
        {
            Rules.RemoveAt(index);
        }
    }
}