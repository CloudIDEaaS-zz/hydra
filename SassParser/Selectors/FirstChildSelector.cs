namespace SassParser

{
    internal sealed class FirstChildSelector : ChildSelector
    {
        public FirstChildSelector(Token token) : base(PseudoClassNames.NthChild, token)
        {
        }
    }
}