namespace SassParser
{
    public interface IConditionFunction : IStylesheetNode
    {
        bool Check();
    }
}