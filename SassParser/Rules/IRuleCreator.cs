namespace SassParser
{
    public interface IRuleCreator
    {
        IRule AddNewRule(RuleType ruleType, Token token);
    }
}